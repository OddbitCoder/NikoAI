FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

ENV TZ=Europe/Ljubljana
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get update

# install utils

RUN apt-get install git -y

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-pip -y

# install Llama

RUN mkdir /app
WORKDIR /app

RUN git clone https://github.com/ggerganov/llama.cpp.git .
# the latest master only supports GGUF models 
#RUN git checkout dadbed99e65252d79f81101a392d0d6497b86caa # this is the last commit that still supports GGML models 
#RUN make clean && LLAMA_CUBLAS=1 make -j # WE NEED GPU SUPPORT TO DO THIS CORRECTLY. WE DO IT IN THE START SCRIPT.
RUN pip install -r requirements.txt 

# python LLAMA

RUN pip install starlette==0.27.*
RUN pip install llama-cpp-python[server]
#RUN pip install llama-cpp-python[server]==0.1.78 # this still supports GGML models

# gRPC

RUN pip install grpcio
RUN pip install grpcio-tools

# all done

ENTRYPOINT ["/files/start-server.sh"]
