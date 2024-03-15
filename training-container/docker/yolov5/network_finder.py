import os
import json

master_address = json.loads(os.environ['SM_HOSTS'])[0]
num_gpus = os.environ['SM_NUM_GPUS']
training_env = json.loads(os.environ['SM_TRAINING_ENV'])

print(master_address)
print(num_gpus)
print(training_env)