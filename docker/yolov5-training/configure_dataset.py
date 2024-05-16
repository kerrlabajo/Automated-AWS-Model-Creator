import yaml
import argparse
import os
import glob

# Define the argument parser
parser = argparse.ArgumentParser(description='Configure dataset')
parser.add_argument('dataset_config_path', type=str, help='The full path to the dataset yaml file')

# Parse the arguments
args = parser.parse_args()

# Extract the dataset name from the file path
DATASET_NAME = os.path.basename(args.dataset_config_path).replace('.yaml', '')

# Define the file path
FILE_PATH = args.dataset_config_path

# Get the directory from the dataset_config_path
dir_path = os.path.dirname(args.dataset_config_path)

# Check if DATASET_NAME.yaml exists
if not os.path.isfile(FILE_PATH):
    # If not, find any .yaml file in the current directory
    yaml_files = glob.glob(os.path.join(dir_path, '*.yaml'))
    if yaml_files:
        # Rename the first .yaml file to DATASET_NAME.yaml
        os.rename(yaml_files[0], FILE_PATH)
    else:
        raise FileNotFoundError("No .yaml file found to rename")

# Open and load the YAML file
with open(FILE_PATH, 'r') as file:
  data = yaml.safe_load(file)

# Check if DATASET_NAME is in the train and val paths
if DATASET_NAME in data['train'] and DATASET_NAME in data['val']:
  # Extract subdirectories after the dataset name in the original paths
  train_subdirs = data['train'].split(DATASET_NAME, 1)[1]
  val_subdirs = data['val'].split(DATASET_NAME, 1)[1]
elif data['train'].startswith('..') and data['val'].startswith('..'):
  # Remove the '..' from the original paths
  train_subdirs = data['train'][2:]
  val_subdirs = data['val'][2:]
else:
  raise ValueError("Invalid format for train or val paths")

# Define the new paths
NEW_PATH = f"/opt/ml/input/data/{DATASET_NAME}"
NEW_TRAIN = f"{NEW_PATH}{train_subdirs}".replace('\\', '/')
NEW_VAL = f"{NEW_PATH}{val_subdirs}".replace('\\', '/')

# Modify the values
data['path'] = NEW_PATH
data['train'] = NEW_TRAIN
data['val'] = NEW_VAL

# Write the changes back to the file
with open(FILE_PATH, 'w') as file:
  yaml.safe_dump(data, file)