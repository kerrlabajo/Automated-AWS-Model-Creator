import os
import json
import yaml
import argparse
import pathlib

# Create the parser
parser = argparse.ArgumentParser(description='Read the Resource Configuration JSON file')

# Add the arguments
parser.add_argument('Path',
          metavar='path',
          type=str,
          help='the path to the json file')

# Execute the parse_args() method
args = parser.parse_args()

# Print the details of the JSON file
with open(args.Path, 'r') as json_file:
  data = json.load(json_file)
  print(data)