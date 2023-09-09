# silence all tqdm progress bars
from tqdm import tqdm
from functools import partialmethod
tqdm.__init__ = partialmethod(tqdm.__init__, disable=True)

from version import __version__
import arrow
from typing import Union, List, Dict, Optional, Any
from pydantic import BaseModel
from time import time
from glob import glob
import yaml
import os
import uuid

import torch
from nemo.core.classes.modelPT import ModelPT
from nemo.utils import logging

import grpc

import contracts_pb2
import contracts_pb2_grpc

import sys
from concurrent import futures

import json


if torch.cuda.is_available() and hasattr(torch.cuda, 'amp') and hasattr(torch.cuda.amp, 'autocast'):
    logging.info("AMP enabled!\n")
    autocast = torch.cuda.amp.autocast
else:
    @contextlib.contextmanager
    def autocast():
        yield

_AUDIO_DURATION_SECONDS_LIMIT = 300
_AUDIO_FILE_SIZE_LIMIT = 44 + _AUDIO_DURATION_SECONDS_LIMIT*16000*2
_use_gpu_if_available = True
_model_tag = "unknown"


class ASRModel(BaseModel):
  class Config:
    arbitrary_types_allowed = True
  tag: str
  nemo: ModelPT
  platform: str
  active: int
  remap: Dict[str,str]

start_time: str = None
models: Dict[str, ASRModel] = {}
num_requests_processed: int = None


class TranscribeResponse(BaseModel):
  result: str


class Model(BaseModel):
  tag: str
  workers: Dict[str,Any]
  features: Optional[Dict[str,Any]]
  info: Optional[Dict[str,Any]]

class Service(contracts_pb2_grpc.WhisperServiceServicer):
  def ProcessAudio(self, request, context): 
    time0 = time()
    dir = "/tmp/"
    if not os.path.exists(dir):
      os.makedirs(dir);
    audio_file_name = dir + str(uuid.uuid4()) + ".wav";
    with open(f"{audio_file_name}", 'wb') as binary_file:
      binary_file.write(request.audioData)

    if _use_gpu_if_available and torch.cuda.is_available():
        models[_model_tag].nemo = models[_model_tag].nemo.cuda()

    models[_model_tag].active += 1
    try:
      with autocast():
        with torch.no_grad():
          transcriptions = models[_model_tag].nemo.transcribe([audio_file_name], batch_size=32)

    except RuntimeError:
      logging.warning("Ran out of memory on device, performing inference on CPU for now")
      try:
        models[_model_tag].nemo = models[_model_tag].nemo.cpu()
        with torch.no_grad():
          transcriptions = models[_model_tag].nemo.transcribe([audio_file_name], batch_size=32)

      except Exception as e:
        models[_model_tag].active -= 1
        logging.error(f"Exception {e} occured while attemting to transcribe audio. Returning error message")
        return f"Exception {e} occured while attemting to transcribe audio." # WARNME

    # If RNNT models transcribe, they return a tuple (greedy, beam_scores)
    if type(transcriptions[0]) == list and len(transcriptions) == 2:
      # get greedy transcriptions only
      transcriptions = transcriptions[0]
    logging.debug(f' T: {transcriptions}')

    # Remap special chars
    for k,v in models[_model_tag].remap.items():
      for i in range(len(transcriptions)):
        transcriptions[i] = transcriptions[i].replace(k,v)
    logging.debug(f' T: {transcriptions}')

    models[_model_tag].active -= 1

    result: TranscribeResponse = { "result": transcriptions[0] }

    transcription_duration = time()-time0
    logging.info(f' R: {audio_file_name}, {result}')
    global num_requests_processed
    num_requests_processed += 1

    if num_requests_processed == 0:
      if _use_gpu_if_available and torch.cuda.is_available():
        # Force onto CPU
        models[_model_tag].nemo = models[_model_tag].nemo.cpu()
        torch.cuda.empty_cache()

    return contracts_pb2.ProcessAudioReply(text=json.dumps(result)) # WARNME


def initialize():
  time0 = time()
  models: Dict[str, ASRModel] = {}
  for _model_info_path in glob(f"./models/**/model.info",recursive=True):
    with open(_model_info_path) as f:
      _model_info = yaml.safe_load(f)

    global _model_tag
    _model_tag = f"{_model_info['language_code']}:{_model_info['domain']}:{_model_info['version']}"
    _model_platform = "gpu" if _use_gpu_if_available and torch.cuda.is_available() else "cpu"
    am=f"{_model_info['info']['am']['framework'].partition(':')[-1].replace(':','_')}.{_model_info['info']['am']['framework'].partition(':')[0]}"
    _model_path=os.path.join(os.path.dirname(_model_info_path),am)

    model = ModelPT.restore_from(_model_path,map_location="cuda" if _model_platform == "gpu" else "cpu")
    model.freeze()
    model.eval()

    models[_model_tag] = ASRModel(
      tag = _model_tag,
      nemo = model,
      platform = _model_platform,
      active = 0,
      remap = _model_info.get('features',[]).get('remap',[])
    )
  logging.info(f'Loaded models {[ (models[model_tag].tag,models[model_tag].platform) for model_tag in models ]}')
  logging.info(f'Initialization finished in {round(time()-time0,2)}s')

  start_time = arrow.utcnow().isoformat()
  num_requests_processed = 0
  return start_time, models, num_requests_processed


if __name__ == "__main__":
  logging.setLevel(logging.DEBUG)
  start_time, models, num_requests_processed = initialize()
  # start RPC service
  port = int(sys.argv[1])
  svc = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
  contracts_pb2_grpc.add_WhisperServiceServicer_to_server(Service(), svc)
  svc.add_insecure_port(f"[::]:{port}")
  svc.start()
  logging.info(f"Service listening on port {port}.")
  svc.wait_for_termination()