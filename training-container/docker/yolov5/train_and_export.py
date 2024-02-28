import shutil
import subprocess
import argparse

def run_script(script, args, use_module=False):
    """
    Run a Python script with arguments.

    Parameters:
    `script` (str): The name of the script to run.
    `args` (list): The arguments to pass to the script.
    `use_module` (bool): Whether to use the -m option to run the script as a module.

    Returns:
    `None`
    """
    if use_module:
        subprocess.run(["python3", "-m", script] + args, check=True)
    else:
        subprocess.run(["python3", script] + args, check=True)
    
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

    return parser.parse_args()

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
    
    converter_args = [
        '/opt/ml/input/config/hyperparameters.json'
    ]
    
    device_count = len(args.device.split(','))  # Count the number of devices
    
    train_args = [
        "torch.distributed.run", 
        "--nproc_per_node", str(device_count),
        "yolov5/train.py", 
        "--img-size", args.img_size, 
        "--batch", args.batch, 
        "--epochs", args.epochs, 
        "--weights", args.weights, 
        "--data", args.data, 
        "--hyp", '/opt/ml/input/config/custom-hyps.yaml' if args.hyp == "Custom" else args.hyp, 
        "--project", args.project, 
        "--name", args.name, 
        "--patience", args.patience, 
        "--workers", args.workers, 
        "--optimizer", args.optimizer, 
        "--device", args.device,
        "--cache"
    ]
    export_args = [
        "--img-size", args.img_size, 
        "--weights", '/opt/ml/output/data/results/weights/best.pt', 
        "--include", args.include, 
        "--device", args.device
    ]

    if args.hyp == "Custom":
        run_script("yolov5/json_to_yaml_converter.py", converter_args)
        
    run_script("-m", train_args)
    run_script("yolov5/export.py", export_args)

    # Copy the best.onnx file to the /opt/ml/model/ directory
    shutil.copy2('/opt/ml/output/data/results/weights/best.onnx', '/opt/ml/model/')

if __name__ == "__main__":
    main()