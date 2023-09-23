## Quick Start

On the host:

1. Copy GGUF models to `\\wsl$\Ubuntu\models`. They need to be on the WSL filesystem otherwise they will load extremely slowly.

2. Start the container:
    ```
    docker-compose up -d
    ```
    Note: Llama.cpp is built when the container is started. Use `docker logs <container ID>` to monitor the build progress. GPU support is required for Llama.cpp to build correctly; hence, it cannot be built when building the image.
3. Enter the container:
    ```
    docker ps
    docker exec -it <container ID> bash
    ```
    
Inside the container:
    
1. Quick test 
    ```
	./main --interactive-first -r "###" --temp 0 -c 2048 -n -1 --ignore-eos --repeat_penalty 1.2 --instruct -m /models/llama-2-7b-chat/llama-2-7b-chat.Q4_0.gguf -ngl 35
    ```

## Useful Links

* Llama 2 models: 
	* 7B: https://huggingface.co/TheBloke/Llama-2-7B-GGUF
	* 13B: https://huggingface.co/TheBloke/Llama-2-13B-GGUF
	* 70B: https://huggingface.co/TheBloke/Llama-2-70B-GGUF
* Llama Chat models:
	* 7B: https://huggingface.co/TheBloke/Llama-2-7b-Chat-GGUF
	* 13B: https://huggingface.co/TheBloke/Llama-2-13B-chat-GGUF
	* 70B: https://huggingface.co/TheBloke/Llama-2-70B-chat-GGUF
* Vicuna models: 
	* 7B: https://huggingface.co/TheBloke/vicuna-7B-v1.5-GGUF
	* 13B: https://huggingface.co/TheBloke/vicuna-13B-v1.5-GGUF
* Alpaca models: ?