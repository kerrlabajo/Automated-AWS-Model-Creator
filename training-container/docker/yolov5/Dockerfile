FROM ubuntu-cuda

RUN git clone https://github.com/ultralytics/yolov5
ENV PATH="/yolov5:${PATH}"
ENV TMPDIR='/var/tmp'

RUN pip3 install -r /yolov5/requirements.txt --no-cache-dir && \
    pip3 install 'onnx>=1.12.0' --no-cache-dir && \
    pip3 install -q --no-input pyyaml
    
COPY json_to_yaml_converter.py /yolov5/json_to_yaml_converter.py
COPY train_and_export.py /yolov5/train_and_export.py
COPY local_ip_address_getter.py /yolov5/local_ip_address_getter.py