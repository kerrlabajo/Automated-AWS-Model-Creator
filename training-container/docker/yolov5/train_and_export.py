import subprocess
import sys

def run_script(script, args):
    """
    Run a Python script with arguments.

    Parameters:
    `script` (str): The name of the script to run.
    `args` (list): The arguments to pass to the script.

    Returns:
    `None`
    """
    subprocess.run(["python3", script] + args, check=True)

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
    train_args = sys.argv[1:25]
    export_args = sys.argv[25:]

    run_script("yolov5/train.py", train_args)
    run_script("yolov5/export.py", export_args)

if __name__ == "__main__":
    main()