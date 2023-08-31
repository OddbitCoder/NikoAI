FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

ENV TZ=Europe/Ljubljana
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get update

# install utils

RUN apt-get install ffmpeg -y
RUN apt-get install git -y

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-pip -y

# install Whisper

RUN mkdir /app
WORKDIR /app

# clone for code inspection
RUN git clone https://github.com/openai/whisper.git .
# install with pip
RUN pip install git+https://github.com/openai/whisper.git 

# gRPC stuff

RUN pip install grpcio
RUN pip install grpcio-tools

# all done

ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]
