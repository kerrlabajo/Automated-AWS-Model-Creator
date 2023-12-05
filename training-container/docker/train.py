import os
import subprocess
import boto3
import yaml
import time
from dotenv import load_dotenv

# Set the environment variable to avoid multithreading conflicts
os.environ['KMP_DUPLICATE_LIB_OK'] = 'True'

# Load environment variables from .env file
load_dotenv()

# Define a method that times the execution of a function
def time_action(method):
  def logger(*args, **kw):
      print('\033[1m' + '\nStarting %r...' % (method.__name__) + '\033[0m')
      ts = time.time()
      result = method(*args, **kw)
      te = time.time()
      print('\033[4m' + '%r completed in ' % (method.__name__) + '\033[92m%2.2f' % (te-ts) + '\033[4m seconds \033[0m\n')
      return result
  return logger

# Download the file
s3.Bucket(os.getenv('S3_BUCKET_NAME')).download_file(os.getenv('S3_FILE_KEY'), 'dataset.zip')

# Extract the zip file
subprocess.run(['unzip', 'dataset.zip', '-d', 'dataset'])

# Define relative paths
data_path = "dataset/data.yaml"
results_path = "dataset/results"
weights_path = "dataset/results/exp/weights/best.pt"

# Load the data from the YAML file
with open(data_path, 'r') as file:
    data = yaml.safe_load(file)

# Update the train, valid, and test paths
HOME = os.getcwd()
data['train'] = HOME + '/dataset/train/images'
data['val'] = HOME + '/dataset/valid/images'
data['test'] = HOME + '/dataset/test/images'

# Write the data back to the YAML file
with open(data_path, 'w') as file:
    yaml.safe_dump(data, file)

# TODO: Set the arguments for the training script from user input/parameters passed
subprocess.run(
  ["python", HOME + "/yolov5/train.py",  "--img", "640", "--batch", "1", "--epochs", "1",
  "--data", "dataset/data.yaml", "--weights", "yolov5s.pt",
  "--project", "dataset/results", "--cache"
  ], check=True
)

# Save the weights path to S3
updated_weights_name = os.urandom(4).hex() + 'best.pt'
s3.meta.client.upload_file(weights_path, os.getenv('S3_BUCKET_NAME'), 'weights/' + updated_weights_name)

# Exit the program
exit(0)
