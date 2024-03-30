FROM ubuntu-cuda

RUN git clone https://github.com/ultralytics/yolov5
ENV PATH="/yolov5:${PATH}"
ENV TMPDIR='/var/tmp'

RUN pip3 install -r /yolov5/requirements.txt --no-cache-dir && \
  pip3 install 'onnx>=1.12.0' --no-cache-dir && \
  pip3 install -q --no-input pyyaml

COPY unittest_yolov5.py /yolov5/unittest_yolov5.py
RUN py -m unittest unittest_yolov5.py

COPY json_to_yaml_converter.py /yolov5/json_to_yaml_converter.py
COPY train_and_export.py /yolov5/train_and_export.py