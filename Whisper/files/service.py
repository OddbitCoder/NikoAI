from concurrent import futures
import statistics
import sys

import grpc

import contracts_pb2
import contracts_pb2_grpc

import torch
import whisper
#from whisper.tokenizer import get_tokenizer

class Service(contracts_pb2_grpc.WhisperServiceServicer):
    def ProcessAudio(self, request, context):  
        #print(len(request.audioData))
        #result = model.transcribe(
        #    audio_path, language=None, temperature=0.0, word_timestamps=True
        #)
        return contracts_pb2.ProcessAudioReply(result=len(request.audioData))

# to GPU or not to GPU

if torch.cuda.is_available():
    device = torch.device("cuda")
    print(f"GPU {torch.cuda.get_device_name(0)} is available.")
else:
    device = torch.device("cpu")
    print("No GPU available, using CPU instead.")

# load the model

print("Loading the model...");

model_name = "medium"
model = whisper.load_model(model_name).to(device)

print("Done.")

# start the service

print("Starting the service...")

port = int(sys.argv[1])
svc = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
contracts_pb2_grpc.add_WhisperServiceServicer_to_server(Service(), svc)
svc.add_insecure_port(f"[::]:{port}")
svc.start()
print(f"Service listening on port {port}.")
svc.wait_for_termination()