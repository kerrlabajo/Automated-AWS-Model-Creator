import shutil
import subprocess
import argparse
import json
import os
import sys
import traceback
import socket
    
def get_node_rank():
    with open('/opt/ml/input/config/resourceconfig.json') as f:
        data = json.load(f)
    current_host = data['current_host']
    hosts = data['hosts']
    node_rank = hosts.index(current_host)
    return current_host, node_rank

def run_script(args, use_module=False):
    """
    Run a Python script with arguments.

    Parameters:
    `args` (list): The script and arguments to pass.
    `use_module` (bool): Whether to use the -m option to run the script as a module.

    Returns:
    `None`
    """
    if use_module:
        subprocess.run(["python3", "-m"] + args, check=True)
    else:
        subprocess.run(["python3"] + args, check=True)
    
def parse_arguments():
    parser = argparse.ArgumentParser(description='Run train.py and export.py scripts with command line arguments.')
    parser.add_argument('--img-size', type=str, required=True)
    parser.add_argument('--batch', type=str, required=True)
    parser.add_argument('--epochs', type=str, required=True)
    parser.add_argument('--weights', type=str, required=True)
    parser.add_argument('--data', type=str, required=True)
    parser.add_argument('--hyp', type=str, required=True)
    parser.add_argument('--project', type=str, required=True)
    parser.add_argument('--name', type=str, required=True)
    parser.add_argument('--patience', type=str, required=True)
    parser.add_argument('--workers', type=str, required=True)
    parser.add_argument('--optimizer', type=str, required=True)
    parser.add_argument('--device', type=str, required=True)
    parser.add_argument('--include', type=str, required=True)
    parser.add_argument('--nnodes', type=str, required=True)
    # parser.add_argument('--node-rank', type=str, required=True)
    # parser.add_argument('--master-addr', type=str, required=True)
    # parser.add_argument('--master_port', type=str, required=True)

    return parser.parse_args()

def main():
    """
    Main function to run `train.py` and `export.py` scripts with command line arguments.

    The first 24 arguments are passed to `train.py` and the remaining arguments are passed to export.py.

    Example:
    >>> python3 /code/train_and_export.py --img-size 640 --batch 1 --epochs 1 --weights yolov5s.pt 
    >>> --data /opt/ml/input/data/train/data.yaml --hyp hyp.scratch-low.yaml 
    >>> --project "/opt/ml/output/data/" --name "results" 
    >>> --patience 100 --workers 8 --optimizer SGD --device 0 --include onnx --nnodes 1

    Returns:
    None
    """
    os.environ["NCCL_DEBUG"] = "INFO"
    os.environ["NCCL_DEBUG_SUBSYS"] = "GRAPH"
    os.environ["NCCL_SOCKET_IFNAME"] = "eth0"
    args = parse_arguments()
    device_count = len(args.device.split(','))
    current_host, node_rank = get_node_rank()
    master_host = 'algo-1'
    master_port = "29500"
    fetched_master_ip = socket.gethostbyname(master_host)
    fetched_local_ip = socket.gethostbyname(current_host)
    
    resource_config_args = [
        "/code/resource_config_reader.py", '/opt/ml/input/config/resourceconfig.json'
    ]
    converter_args = [
        "/code/json_to_yaml_converter.py", '/opt/ml/input/config/hyperparameters.json'
    ]
    multi_gpu_ddp_args = [
        "torch.distributed.run", "--nproc_per_node", str(device_count)
    ]
    multi_instance_gpu_ddp_args = [
        "torch.distributed.run", "--nproc_per_node", str(device_count), 
        "--nnodes", args.nnodes, "--node_rank", str(node_rank), 
        "--master_addr", master_host, "--master_port", master_port
    ]
    train_args = [
        "/code/yolov5/train.py", "--img-size", args.img_size, "--batch", args.batch, "--epochs", args.epochs, 
        "--weights", args.weights, "--data", args.data, 
        "--hyp", '/opt/ml/input/config/custom-hyps.yaml' if args.hyp == "Custom" else args.hyp, 
        "--project", args.project, "--name", args.name, 
        "--patience", args.patience, "--workers", args.workers, "--optimizer", args.optimizer, 
        "--device", args.device, "--cache", "--exist-ok"
    ]
    export_args = [
        "/code/yolov5/export.py", "--img-size", args.img_size, 
        "--weights", args.project + args.name + '/weights/best.pt', 
        "--include", args.include, "--device", args.device
    ]
    
    print("Master IP address:", fetched_master_ip)
    print("Local IP address:", fetched_local_ip)
    
    run_script(resource_config_args)
    
    run_script(converter_args) if args.hyp == "Custom" else None
        
    if int(args.nnodes) > 1:
        run_script(multi_instance_gpu_ddp_args + train_args, use_module=True)
          
    if device_count > 1:
        run_script(multi_gpu_ddp_args + train_args, use_module=True)
    else:
        run_script(train_args)
        
    run_script(export_args)

    # Copy the best.onnx file to the /opt/ml/model/ directory
    shutil.copy2('/opt/ml/output/data/results/weights/best.onnx', '/opt/ml/model/')

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        with open('/opt/ml/output/failure', 'w') as f:
            print(e)
            f.write(str(e))
            f.write(traceback.format_exc())
        sys.exit(1)