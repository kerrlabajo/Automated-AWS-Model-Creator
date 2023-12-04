FROM ubuntu:20.04
FROM python:3.10-bookworm

LABEL maintainer="Amazon AI <sage-learner@amazon.com>"

RUN apt-get update && apt-get install ffmpeg libsm6 libxext6  -y
RUN apt-get install -y git
RUN pip install --upgrade pip

RUN pip install supabase 'boto3>1.0<2.0' 'sagemaker>2.0<3.0' --quiet --no-cache-dir

RUN git clone https://github.com/ultralytics/yolov5
RUN pip install -r yolov5/requirements.txt --no-cache-dir

ENV PYTHONUNBUFFERED=TRUE
ENV PYTHONDONTWRITEBYTECODE=TRUE

COPY lsc-inspector ./lsc-inspector
COPY training_algorithm.py ./training_algorithm.py

CMD [ "python", "training_algorithm.py" ]