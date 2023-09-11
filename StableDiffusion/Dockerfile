FROM nvidia/cuda:12.2.0-devel-ubuntu20.04

RUN apt-get update

RUN apt-get install curl -y

# install Miniconda

WORKDIR /root
RUN curl https://repo.anaconda.com/miniconda/Miniconda3-latest-Linux-x86_64.sh -o Miniconda3-latest-Linux-x86_64.sh
RUN chmod +x Miniconda3-latest-Linux-x86_64.sh
RUN ./Miniconda3-latest-Linux-x86_64.sh -b
ENV PATH="${PATH}:/root/miniconda3/bin"

# install Python

RUN apt-get install python3 -y
RUN apt-get install python3-venv -y
RUN apt-get install python3-pip -y

# install tools

ENV TZ=Europe/Ljubljana
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get install git -y
RUN apt-get install ffmpeg libsm6 libxext6 -y
RUN apt-get install google-perftools -y

# install Stable Diffusion Web UI

WORKDIR /root/sd
RUN git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui.git .
RUN chmod +x webui.sh
RUN cp launch.py launch.py.bak
RUN sed -i 's/start()//g' launch.py
RUN ./webui.sh -f --skip-torch-cuda-test --xformers
RUN rm launch.py && mv launch.py.bak launch.py

# install ControlNet deps

RUN venv/bin/python3 -m pip install mediapipe svglib fvcore --prefer-binary

# all done

#ENTRYPOINT ["/root/sd/webui.sh"]
#CMD ["-f","--listen","--enable-insecure-extension-access","--xformers"]

ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]
