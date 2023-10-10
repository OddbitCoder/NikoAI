## Quick Start

### Server Mode

1. Start the container:
    ```
    docker-compose up -d
    ```

This starts the gRPC server on 9010.

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
    
1. Quick test (transcribes `/files/jfk.flac`):
    ```
    /files/test.sh # test command-line tool
    python3 /files/test.py # test python code
    ```
2. Start the gRPC server on 9010:
    ```
    /files/start_service.sh 9010
    ```

## Useful Links

* Whisper project: https://github.com/openai/whisper