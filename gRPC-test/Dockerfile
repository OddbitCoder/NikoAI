FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

RUN apt-get update

# install utils

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-pip -y

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
