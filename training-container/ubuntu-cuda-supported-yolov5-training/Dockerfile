# Start from NVIDIA CUDA image
FROM nvidia/cuda:12.3.2-base-ubuntu22.04

# Avoid timezone prompts
ENV DEBIAN_FRONTEND=noninteractive

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
    net-tools \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Clone the repository and set the PATH
RUN mkdir /code && git clone https://github.com/ultralytics/yolov5 /code/yolov5
ENV PATH="/code:${PATH}"

# Install Python dependencies
RUN pip3 install -r /code/yolov5/requirements.txt --no-cache-dir && \
    pip3 install 'onnx>=1.12.0' --no-cache-dir && \
    pip3 install -q --no-input pyyaml

# Copy the Python scripts
COPY ../ubuntu-cuda-supported-yolov5-training/json_to_yaml_converter.py /code/json_to_yaml_converter.py
COPY ../ubuntu-cuda-supported-yolov5-training/train_and_export.py /code/train_and_export.py
COPY ../ubuntu-cuda-supported-yolov5-training/resource_config_reader.py /code/resource_config_reader.py