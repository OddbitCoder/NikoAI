FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

#ENV TZ=Europe/Ljubljana
#RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get update

# install utils

#RUN apt-get install libsndfile1-dev -y
#RUN apt-get install git -y

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-pip -y

# install Pyannote.Audio

RUN mkdir /app
WORKDIR /app

# gRPC stuff

RUN pip install grpcio
RUN pip install grpcio-tools

# all done

#ENTRYPOINT ["tail"]
#CMD ["-f","/dev/null"]

ENTRYPOINT ["python3"]
CMD ["/files/service.py","9010"]
