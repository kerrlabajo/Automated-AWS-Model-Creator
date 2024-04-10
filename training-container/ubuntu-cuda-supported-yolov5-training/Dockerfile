# Start from NVIDIA CUDA image
FROM nvidia/cuda:12.3.2-base-ubuntu22.04

# Avoid timezone prompts
ENV DEBIAN_FRONTEND=noninteractive

# Update and install dependencies
RUN apt-get update --fix-missing && \
    apt-get upgrade -y && \
    apt-get install -y --no-install-recommends \
    autoconf \
    apt-utils \
    pciutils \
    environment-modules \
    tcl \
    libnl-3-200 \
    libnl-3-dev \
    libnl-route-3-200 \
    libnl-route-3-dev \
    rdma-core \
    udev \
    dmidecode \
    ethtool \
    iproute2 \
    libevent-core-2.1-7 \
    libevent-pthreads-2.1-7 \
    libhwloc15 \
    libatomic1 \
    openssh-client \
    ssh-client \
    ca-certificates \
    gnupg \
    curl \
    git \
    python3 \
    python3-pip \
    ffmpeg libsm6 libxext6 \
    net-tools \
    make \
    g++ \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN cd - && mkdir /code && git clone https://github.com/ultralytics/yolov5 /code/yolov5
ENV PATH="/code:${PATH}"

# Install Python dependencies
RUN pip3 install -r /code/yolov5/requirements.txt --no-cache-dir && \
    pip3 install 'onnx>=1.12.0' --no-cache-dir && \
    pip3 install -q --no-input pyyaml

# Copy the Python scripts
COPY json_to_yaml_converter.py /code/json_to_yaml_converter.py
COPY train_and_export.py /code/train_and_export.py
COPY resource_config_reader.py /code/resource_config_reader.py