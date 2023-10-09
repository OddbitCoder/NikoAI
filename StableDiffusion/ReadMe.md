## Quick Start

### User Mode

1. Start the container:
    ```
    docker-compose up -d
    ```

In your browser, navigate to `http://localhost:7860`.

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
    
1. Start the UI server (on port 7860):
    ```
    /root/sd/webui.sh -f --listen --enable-insecure-extension-access --xformers
    ```

In your browser, navigate to `http://localhost:7860`.

## Useful Links

* Install ControlNet: https://www.youtube.com/watch?v=uUizoFA7OYY