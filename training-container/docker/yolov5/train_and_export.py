import shutil
import subprocess
import argparse
import os
import re

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
    # parser.add_argument('--nnodes', type=str, required=True)
    # parser.add_argument('--node-rank', type=str, required=True)
    # parser.add_argument('--master-addr', type=str, required=True)
    # parser.add_argument('--master_port', type=str, required=True)

    return parser.parse_args()

def get_latest_folder(base_folder):
    """
    Get the latest folder that starts with the base_folder name.

    Parameters:
    `base_folder` (str): The base folder name.

    Returns:
    `str`: The latest folder name.
    """
    all_dirs = next(os.walk('/opt/ml/output/data/'))[1]

    matching_dirs = [d for d in all_dirs if d.startswith(base_folder)]
    # print all matching dirs
    print(matching_dirs)

    if not matching_dirs:
        print("No matching directories found.")
        return base_folder

    numbers = [int(re.search(r'\d+$', d).group()) for d in matching_dirs if re.search(r'\d+$', d)]

    if not numbers:
        print("No numbers found in matching directories.")
        return base_folder

    highest_number = max(numbers)

    return base_folder + str(highest_number)

def main():
    """
    Main function to run `train.py` and `export.py` scripts with command line arguments.

    The first 24 arguments are passed to `train.py` and the remaining arguments are passed to export.py.

    Example:
    >>> python3 yolov5/train_and_export.py --img-size 640 --batch 1 --epochs 1 --weights yolov5s.pt 
    >>> --data /opt/ml/input/data/train/data.yaml --hyp hyp.scratch-low.yaml 
    >>> --project "/opt/ml/output/data/" --name "results" 
    >>> --patience 100 --workers 8 --optimizer SGD --device 0 
    >>> --img-size 640 --weights /opt/ml/output/data/results/weights/best.pt --include onnx --device 0
    
    The 25th args is the start of the `export.py` args.

    Returns:
    None
    """
    args = parse_arguments()
    device_count = len(args.device.split(','))
    
    converter_args = [
        "yolov5/json_to_yaml_converter.py", '/opt/ml/input/config/hyperparameters.json'
    ]
    multi_gpu_ddp_args = [
        "torch.distributed.run", "--nproc_per_node", str(device_count)
    ]
    # multi_instance_gpu_ddp_args = [
    #     "torch.distributed.run", "--nproc_per_node", str(device_count), 
    #     "--nnodes", args.nnodes, "--node_rank", args.node_rank, 
    #     "--master_addr", args.master_addr, "--master_port", args.master_port
    # ]
    train_args = [
        "yolov5/train.py", "--img-size", args.img_size, "--batch", args.batch, "--epochs", args.epochs, 
        "--weights", args.weights, "--data", args.data, 
        "--hyp", '/opt/ml/input/config/custom-hyps.yaml' if args.hyp == "Custom" else args.hyp, 
        "--project", args.project, "--name", args.name, 
        "--patience", args.patience, "--workers", args.workers, "--optimizer", args.optimizer, 
        "--device", args.device, "--cache"
    ]
    export_args = [
        "yolov5/export.py", "--img-size", args.img_size, 
        "--weights", args.project + get_latest_folder(args.name) + '/weights/best.pt', 
        "--include", args.include, "--device", args.device
    ]

    run_script(converter_args) if args.hyp == "Custom" else None
        
    if device_count > 1:
        run_script(multi_gpu_ddp_args + train_args, use_module=True)
    else:
        run_script(train_args)
        
    run_script(export_args)

    # Copy the best.onnx file to the /opt/ml/model/ directory
    shutil.copy2('/opt/ml/output/data/results/weights/best.onnx', '/opt/ml/model/')

if __name__ == "__main__":
    main()