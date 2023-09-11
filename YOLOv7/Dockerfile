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

# install Yolo

RUN mkdir /app
WORKDIR /app

RUN git clone https://github.com/WongKinYiu/yolov7.git .

RUN pip install urllib3==1.26.16
RUN pip install -r requirements.txt

# all done

ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]
