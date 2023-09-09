FROM nvcr.io/nvidia/nemo:22.07 as nemo

ARG DEBIAN_FRONTEND=noninteractive

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install ffmpeg -y

FROM nemo as service

ARG DEBIAN_FRONTEND=noninteractive

COPY . /opt/asr
RUN python3 -m pip install -r /opt/asr/requirements.txt
WORKDIR /opt/asr

RUN pip install grpcio
RUN pip install grpcio-tools

# WARNME: this fixes some ver issues (is there a better way?)

RUN pip install pydantic==1.*
RUN pip install protobuf==3.20.*

#ENTRYPOINT [ "python3", "/files/server.py", "9010" ]
ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]