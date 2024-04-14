import os
import json
import yaml
import argparse
import pathlib

# Create the parser
parser = argparse.ArgumentParser(description='Convert JSON to YAML')

# Add the arguments
parser.add_argument('Path',
          metavar='path',
          type=str,
          help='the path to the json file')

# Execute the parse_args() method
args = parser.parse_args()

# Load the JSON file
with open(args.Path, 'r') as json_file:
  data = json.load(json_file)

# Convert all values to float
for key in data:
  data[key] = float(data[key])
  
# Get the directory of the input file
input_dir = pathlib.Path(args.Path).parent

# Create the output file path
output_file_path = os.path.join(input_dir, 'custom-hyps.yaml')

# Write the data to the YAML file
with open(output_file_path, 'w') as yaml_file:
    yaml.dump(data, yaml_file, sort_keys=False)