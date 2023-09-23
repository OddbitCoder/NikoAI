#!/bin/sh

if [ -f "main" ]; then
    echo "Already initialized."
else
	echo "Not initialized. Initializing..." 
	make clean && LLAMA_CUBLAS=1 make -j
	echo "Done."
fi

tail -f /dev/null