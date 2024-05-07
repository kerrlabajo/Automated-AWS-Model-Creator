import yaml
import argparse
import os

# Define the argument parser
parser = argparse.ArgumentParser(description='Configure dataset')
parser.add_argument('dataset_config_path', type=str, help='The full path to the dataset yaml file')

# Parse the arguments
args = parser.parse_args()

# Extract the dataset name from the file path
DATASET_NAME = os.path.basename(args.dataset_config_path).replace('.yaml', '')

# Define the file path
FILE_PATH = args.dataset_config_path

# Define the new paths
NEW_PATH = f"/opt/ml/input/data/{DATASET_NAME}"
NEW_TRAIN = f"{NEW_PATH}/images/train"
NEW_VAL = f"{NEW_PATH}/images/train"

# Open and load the YAML file
with open(FILE_PATH, 'r') as file:
  data = yaml.safe_load(file)

# Modify the values
data['path'] = NEW_PATH
data['train'] = NEW_TRAIN
data['val'] = NEW_VAL

# Write the changes back to the file
with open(FILE_PATH, 'w') as file:
  yaml.safe_dump(data, file)