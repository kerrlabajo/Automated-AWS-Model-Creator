import yaml
import argparse

# Define the argument parser
parser = argparse.ArgumentParser(description='Configure dataset')
parser.add_argument('dataset_name', type=str, help='The name of the dataset')

# Parse the arguments
args = parser.parse_args()

# Define the dataset name and file path
DATASET_NAME = args.dataset_name
FILE_PATH = f"/opt/ml/input/data/{DATASET_NAME}/{DATASET_NAME}.yaml"

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