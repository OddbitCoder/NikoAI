## Quick Start

On the host:

1. Start the container:
    ```
    docker-compose up -d
    ```
2. "Enter" the container:
    ```
    docker ps
    docker exec -it <container ID> bash
    ```
    
Inside the container:
    
1. Quick test (transcribes `/files/jfk.flac`):
    ```
    /files/test.sh # <- test command-line tool
    python3 /files/test.py # <- test python code
    ```
2. Start the gRPC server (on port 9010):
    ```
    /files/start_service.sh 9010
    ```

## Useful Links

* Speech-to-Text Benchmark: https://huggingface.co/spaces/autoevaluate/leaderboards?dataset=mozilla-foundation%2Fcommon_voice_11_0&only_verified=0&task=automatic-speech-recognition
* Slovenian model: https://huggingface.co/DrishtiSharma/whisper-large-v2-slovenian
* Convert fine-tuned models to Whisper.CPP format: https://github.com/ggerganov/whisper.cpp/tree/master/models#fine-tuned-models
* gRPC: 
    * https://aiki.dev/posts/csharp-grpc-basic/
    * https://aiki.dev/posts/csharp-grpc-secure/
    * https://aiki.dev/posts/csharp-grpc-streaming/
    * https://aiki.dev/posts/csharp-grpc-interop/