FROM 763104351884.dkr.ecr.ap-southeast-1.amazonaws.com/pytorch-training:2.2.0-gpu-py310-cu121-ubuntu20.04-sagemaker

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