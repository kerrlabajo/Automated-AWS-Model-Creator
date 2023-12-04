FROM ubuntu:20.04
FROM python:3.10-bookworm

LABEL maintainer="Amazon AI"

RUN apt-get update && apt-get install ffmpeg libsm6 libxext6  -y
RUN apt-get install -y git
RUN apt-get install unzip
RUN pip install --upgrade pip

RUN pip install subprocess boto3 python-dotenv 'boto3>1.0<2.0' 'sagemaker>2.0<3.0' --quiet --no-cache-dir

RUN git clone https://github.com/ultralytics/yolov5 /opt/ml/code/yolov5
RUN pip install -r /opt/ml/code/yolov5/requirements.txt --no-cache-dir

ENV PYTHONUNBUFFERED=TRUE
ENV PYTHONDONTWRITEBYTECODE=TRUE

# copy the training script inside the container
COPY train.py /opt/ml/code/train.py
COPY .env /opt/ml/code/.env

# define train.py as the script entry point
ENV SAGEMAKER_PROGRAM train.py