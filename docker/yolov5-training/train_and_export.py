import shutil
import subprocess
import argparse
import json
import sys
import traceback


def get_hosts_and_node_rank():
    """
    This function reads the resource configuration file provided by SageMaker
    and returns the current host, its rank among all hosts (node rank), and the list of all hosts.

    The resource configuration file is a JSON file that has information about the current
    and all hosts. It's located at '/opt/ml/input/config/resourceconfig.json'.

    Returns:
        current_host (str): The name of the current host.
        node_rank (int): The rank of the current host in the list of all hosts.
    """
    with open("/opt/ml/input/config/resourceconfig.json") as f:
        data = json.load(f)
    current_host = data["current_host"]
    hosts = data["hosts"]
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
    parser = argparse.ArgumentParser(
        description="Run train.py and export.py scripts with command line arguments."
    )
    parser.add_argument("--img-size", type=str, required=True)
    parser.add_argument("--batch", type=str, required=True)
    parser.add_argument("--epochs", type=str, required=True)
    parser.add_argument("--weights", type=str, required=True)
    parser.add_argument("--data", type=str, required=True)
    parser.add_argument("--hyp", type=str, required=True)
    parser.add_argument("--project", type=str, required=True)
    parser.add_argument("--name", type=str, required=True)
    parser.add_argument("--patience", type=str, required=True)
    parser.add_argument("--workers", type=str, required=True)
    parser.add_argument("--optimizer", type=str, required=True)
    parser.add_argument("--device", type=str, required=True)
    parser.add_argument("--include", type=str, required=True)
    parser.add_argument("--nnodes", type=str, required=True)
    # parser.add_argument('--node-rank', type=str, required=True)
    # parser.add_argument('--master-addr', type=str, required=True)
    # parser.add_argument('--master_port', type=str, required=True)

    return parser.parse_args()


def main():
    """
    A main entrypoint located in `/code` directory to run the `train.py` and `export.py` located in `/code/yolov5`. 
    This entrypoint can only be accessed by the `AWS SDK for .NET` using the `CreateTrainingJob API` operation. 
    By providing the `ContainerEntrypoint` and `ContainerArguments`, this script will be executed.
    
    Parameters:
    `--img-size` (str): Image size for training.
    `--batch` (str): Batch size for training.
    `--epochs` (str): Number of epochs for training.
    `--weights` (str): Pre-trained weights for training.
    `--data` (str): Path to the data configuration file.
    `--hyp` (str): Path to the hyperparameters configuration file.
    `--project` (str): Path to the resulting directory.
    `--name` (str): Name of the results.
    `--patience` (str): Early stopping patience.
    `--workers` (str): Number of workers for DataLoader.
    `--optimizer` (str): Optimizer for training.
    `--device` (str): Device for training (cpu/gpus).
    `--include` (str): File format for exporting from PyTorch file.
    `--nnodes` (str): Number of nodes (instances/machine) for distributed training.

    Example:
    >>> train_and_export.py --img-size 640 --batch 1 --epochs 1 --weights yolov5s.pt
    >>> --data /opt/ml/input/data/<dataset_name>/<dataset_name>.yaml --hyp hyp.scratch-low.yaml
    >>> --project "/opt/ml/output/data/" --name "results"
    >>> --patience 100 --workers 8 --optimizer SGD --device 0 --include onnx --nnodes 1

    Returns:
    `None`: If exit code returns 0, the script runs successfully, 1 if an exception occurs.
    """
    args = parse_arguments()
    device_count = len(args.device.split(","))
    current_host, node_rank = get_hosts_and_node_rank()
    master_host = "algo-1"
    master_port = "29500"

    converter_args = [
        "/code/json_to_yaml_converter.py", "/opt/ml/input/config/hyperparameters.json",
    ]
    configure_dataset_args = [
        "/code/configure_dataset.py", args.data,
    ]
    multi_gpu_ddp_args = [
        "torch.distributed.run", "--nproc_per_node", str(device_count),
    ]
    multi_node_gpu_ddp_args = [
        "torch.distributed.run", "--nproc_per_node", str(device_count),
        "--nnodes", args.nnodes, "--node_rank", str(node_rank),
        "--master_addr", master_host, "--master_port", master_port,
    ]
    train_args = [
        "/code/yolov5/train.py", "--img-size", args.img_size, 
        "--batch", args.batch,  "--epochs", args.epochs, "--weights", args.weights, 
        "--data", args.data, "--hyp", "/opt/ml/input/config/custom-hyps.yaml" if args.hyp == "Custom" else args.hyp,
        "--project", args.project, "--name", args.name,
        "--patience", args.patience, "--workers", args.workers, "--optimizer", args.optimizer,
        "--device", args.device, "--cache", "--exist-ok",
    ]
    export_args = [
        "/code/yolov5/export.py", "--img-size", args.img_size,
        "--weights", args.project + args.name + "/weights/best.pt",
        "--include", args.include, "--device", args.device,
        "--opset", "12",
    ]

    run_script(converter_args) if args.hyp == "Custom" else None
    run_script(configure_dataset_args)

    if int(args.nnodes) > 1:
        run_script(multi_node_gpu_ddp_args + train_args, use_module=True)
    elif device_count > 1:
        run_script(multi_gpu_ddp_args + train_args, use_module=True)
    else:
        run_script(train_args)

    if current_host == master_host:
        run_script(export_args)
        shutil.copy2("/opt/ml/output/data/results/weights/best.onnx", "/opt/ml/model/")


if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        with open("/opt/ml/output/failure", "w") as f:
            print(e)
            f.write(str(e))
            f.write(traceback.format_exc())
        sys.exit(1)
