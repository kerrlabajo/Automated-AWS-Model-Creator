# Start from NVIDIA CUDA image
FROM nvidia/cuda:12.4.1-base-ubuntu22.04

# Avoid timezone prompts
ENV DEBIAN_FRONTEND=noninteractive \
  NCCL_SOCKET_IFNAME=eth0

# Update and install dependencies
RUN apt-get update --fix-missing && \
  apt-get upgrade -y && \
  apt-get install -y --no-install-recommends \
  apt-utils \
  ca-certificates \
  gnupg \
  curl \
  git \
  python3 \
  python3-pip \
  ffmpeg libsm6 libxext6 \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*

# Clone the repository and set the PATH
RUN mkdir /code && git clone https://github.com/ultralytics/yolov5 /code/yolov5
ENV PATH="${PATH}:/code"

# Install Python dependencies
RUN pip3 install -r /code/yolov5/requirements.txt --no-cache-dir && \
  pip3 install 'onnx>=1.10.0' --no-cache-dir && \
  pip3 install -q --no-input pyyaml --no-cache-dir

# Copy the Python scripts
COPY json_to_yaml_converter.py /code/json_to_yaml_converter.py
COPY configure_dataset.py /code/configure_dataset.py
COPY train_and_export.py /code/train_and_export.py