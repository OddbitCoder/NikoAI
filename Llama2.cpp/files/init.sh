#!/bin/sh

if [ -f "main" ]; then
    echo "Already built."
else
    echo "Not built. Building..." 
    make clean && LLAMA_CUBLAS=1 make -j
    echo "Done."
fi