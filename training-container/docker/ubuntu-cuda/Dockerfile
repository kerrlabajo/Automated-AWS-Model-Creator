# Start from NVIDIA CUDA image
FROM nvidia/cuda:12.3.1-base-ubuntu20.04

# Avoid timezone prompts
ENV DEBIAN_FRONTEND=noninteractive

# Update and install dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    gnupg \
    curl \
    git \
    python3 \
    python3-pip \
    ffmpeg libsm6 libxext6

# Clean up
RUN apt-get clean && \
    rm -rf /var/lib/apt/lists/*