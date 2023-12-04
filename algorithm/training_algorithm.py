import os
import subprocess
from supabase import create_client, Client

# Set the environment variable to avoid multithreading conflicts
os.environ['KMP_DUPLICATE_LIB_OK'] = 'True'

# TODO: Retrieve the dataset.zip from S3 and extract it to the lsc-inspector folder


# Define relative paths
data_path = "lsc-inspector/data.yaml"
results_path = "lsc-inspector/results"
weights_path = "lsc-inspector/results/exp/weights/best.pt"
supabase_url = 'https://tgbqbnhnyjakucemhefo.supabase.co/'
key = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRnYnFibmhueWpha3VjZW1oZWZvIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTQ3NTg1NjQsImV4cCI6MjAxMDMzNDU2NH0.wxVjFK5HBbOwTpe7fbd7Sl6pqAzZnkPloTqxkFPb9RI'
supabase: Client = create_client(supabase_url, key)


# Extract the lsc-inspector zip file as a folder called lsc-inspector the same directory as this current train.py file and the yolov5 folder
# The tree structure should look like this:
# .
# |── train.py
# |── yolov5/
# ├── lsc-inspector/
# │   ├── data.yaml
# │   ├── train
# │   ├── valid
# │   ├── test
# │   ├── README.dataset.txt
# │   ├── README.roboflow.txt


# Try to retrieve the model at "lsc-inspector/results/exp/weights/best.pt"
# Train and export the model as ONNX
def train():
  res = subprocess.run(
    ["python", "yolov5/train.py",  "--img", "640", "--batch", "1", "--epochs", "2",
    "--data", "lsc-inspector/data.yaml", "--weights", "yolov5s.pt",
    "--project", "lsc-inspector/results", "--cache"
    ]
  )

# Upload the model to S3
def upload_to_supabase():
  custom_file_name = os.urandom(4).hex() + "best.pt"
  with open(weights_path , 'rb') as f:
    supabase.storage.from_('lsc_weights').upload(file=f, path=custom_file_name)

if __name__ == '__main__':
  train()
  upload_to_supabase()