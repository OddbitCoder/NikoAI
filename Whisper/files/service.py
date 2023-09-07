# transcribe directly from byte stream: https://github.com/openai/whisper/discussions/908

from concurrent import futures
import sys

import grpc

import contracts_pb2
import contracts_pb2_grpc

import torch
import whisper

import signal

import os
import uuid

import json

from whisper.model import ModelDimensions, Whisper

# set Ctrl+C handler

def handler(signum, frame):
    exit(1)

signal.signal(signal.SIGINT, handler)

# to GPU or not to GPU

if torch.cuda.is_available():
    device = torch.device("cuda")
    print(f"GPU {torch.cuda.get_device_name(0)} is available.")
else:
    device = torch.device("cpu")
    print("No GPU available, using CPU instead.")

# load the model

print("Loading the model...");

model_name = "small" # 'tiny.en', 'tiny', 'base.en', 'base', 'small.en', 'small', 'medium.en', 'medium', 'large-v1', 'large-v2', 'large'
model = whisper.load_model(model_name)#.to(device) # the last part is done in load_model

print("Done.")

# start the service

print("Starting the service...")

lang="en"

class Service(contracts_pb2_grpc.WhisperServiceServicer):
    def ProcessAudio(self, request, context): 
        # At this moment, this function saves the bytes into a temp file and then uses the model to open this file. 
        # It would be more efficient if this function would receive "raw" bytes and send those directly to the model.
        dir = "/root/.tmp/"
        if not os.path.exists(dir):
            os.makedirs(dir);
        audio_file_name = dir + str(uuid.uuid4()) + ".wav";
        with open(audio_file_name, "wb") as binary_file:
            binary_file.write(request.audioData)
        result = model.transcribe(
           audio_file_name, language=lang, temperature=0.0, word_timestamps=True 
        )
        #print(result);
        os.remove(audio_file_name)
        return contracts_pb2.ProcessAudioReply(text=json.dumps(result))

port = int(sys.argv[1])
svc = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
contracts_pb2_grpc.add_WhisperServiceServicer_to_server(Service(), svc)
svc.add_insecure_port(f"[::]:{port}")
svc.start()

# wait for termination

print(f"Service listening on port {port}.")
svc.wait_for_termination()
