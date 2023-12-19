FROM ubuntu-cuda

RUN git clone https://github.com/ultralytics/yolov5
ENV PATH="/yolov5:${PATH}"
RUN pip3 install -r /yolov5/requirements.txt --no-cache-dir
RUN pip3 install 'onnx>=1.12.0' --no-cache-dir
# Need this for Automated Model Tuning support
RUN pip3 install -q --no-input pyyaml