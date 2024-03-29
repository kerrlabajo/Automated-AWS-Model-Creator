FROM ubuntu-cuda

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