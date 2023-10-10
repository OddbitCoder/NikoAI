## Quick Start

### Server Mode

1. Start the container:
    ```
    docker-compose up -d
    ```

This starts the gRPC server on TBD.

### Dev Mode

On the host:

1. Start the container:
    ```
    docker-compose -f docker-compose.dev.yml up -d
    ```
2. Enter the container:
    ```
    docker ps
    docker exec -it <container ID> bash
    ```
    
Inside the container:

1. Quick test:
    ```
    python3 /files/test.py
    ```
2. Start the gRPC server on TBD:
    ```
    TBD
    ```

## Useful Links

* GitHub repo: https://github.com/pyannote/pyannote-audio
* Tutorial 1: https://lablab.ai/t/whisper-transcription-and-speaker-identification
* Tutorial 2: https://www.youtube.com/watch?v=MVW746z8y_I