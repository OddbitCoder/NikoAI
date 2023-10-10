#!/bin/sh

python3 /files/inf/detect.py --weights /models/yolov7.pt --no-trace --conf 0.25 --img-size 640 --source /files/in/horses.jpg --project /files/out --name "" --exist-ok