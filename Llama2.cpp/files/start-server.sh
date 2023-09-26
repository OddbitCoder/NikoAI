#!/bin/sh

/files/init.sh

# HTTP server mode (navigate to http://localhost:9015/docs)
python3 -m llama_cpp.server --model /models/llama-2-7b-chat/llama-2-7b-chat.Q4_0.gguf --host 0.0.0.0 --port 9015
