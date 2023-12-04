import os
import subprocess
import boto3
import yaml
from dotenv import load_dotenv

# Set the environment variable to avoid multithreading conflicts
os.environ['KMP_DUPLICATE_LIB_OK'] = 'True'

# Load environment variables from .env file
load_dotenv()

# Create an s3 resource object
s3 = boto3.resource('s3', 
     aws_access_key_id=os.getenv('ACCESS_KEY_ID'), 
     aws_secret_access_key=os.getenv('SECRET_ACCESS_KEY'))

# Download the file
s3.Bucket(os.getenv('S3_BUCKET_NAME')).download_file(os.getenv('S3_FILE_KEY'), 'dataset.zip')

# Extract the zip file
subprocess.run(['unzip', 'dataset.zip', '-d', 'dataset'])

# Define relative paths
data_path = "dataset/data.yaml"
results_path = "dataset/results"
weights_path = "dataset/results/exp/weights/best.pt"


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