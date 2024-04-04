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
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

ARG OPEN_MPI_PATH=/opt/amazon/openmpi/
ENV NCCL_VERSION=2.7.8
ENV EFA_VERSION=1.30.0
ENV BRANCH_OFI=1.1.1

#################################################
## EFA and MPI SETUP
RUN cd $HOME \
  && curl -O https://s3-us-west-2.amazonaws.com/aws-efa-installer/aws-efa-installer-${EFA_VERSION}.tar.gz \
  && tar -xf aws-efa-installer-${EFA_VERSION}.tar.gz \
  && cd aws-efa-installer \
  && ./efa_installer.sh -y --skip-kmod -g

ENV PATH="$OPEN_MPI_PATH/bin:$PATH"
ENV LD_LIBRARY_PATH="$OPEN_MPI_PATH/lib/:$LD_LIBRARY_PATH"
  
RUN cd $HOME \
  && git clone https://github.com/NVIDIA/nccl-tests \
  && cd nccl-tests \
  && make MPI=1 MPI_HOME=/opt/amazon/openmpi CUDA_HOME=/usr/local/cuda NCCL_HOME=/usr/local

# Clone the repository and set the PATH
RUN mkdir /code && git clone https://github.com/ultralytics/yolov5 /code/yolov5
ENV PATH="/code:${PATH}"

# Install Python dependencies
RUN pip3 install -r /code/yolov5/requirements.txt --no-cache-dir && \
    pip3 install 'onnx>=1.12.0' --no-cache-dir && \
    pip3 install -q --no-input pyyaml

# Copy the Python scripts
COPY json_to_yaml_converter.py /code/json_to_yaml_converter.py
COPY train_and_export.py /code/train_and_export.py
COPY resource_config_reader.py /code/resource_config_reader.py