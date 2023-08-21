FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

ENV TZ=Europe/Belgrade
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get update

# install utils

RUN apt-get install libsndfile1-dev -y
RUN apt-get install git -y

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-pip -y

# install Pyannote.Audio

RUN mkdir /app
WORKDIR /app

# clone for code inspection
RUN git clone https://github.com/pyannote/pyannote-audio.git .
# install with pip
RUN pip install starlette==0.27.0
RUN pip install https://github.com/pyannote/pyannote-audio/archive/refs/heads/develop.zip
RUN pip install torchvision 

# all done

ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]
