import unittest
from unittest.mock import patch, MagicMock
import subprocess
from train_and_export import run_script 

class TestTrainAndExport(unittest.TestCase):

    @patch('subprocess.run')
    def test_run_script_file_not_found_error(self, mock_subprocess_run):
        # Mock the subprocess.run to raise a FileNotFoundError
        mock_subprocess_run.side_effect = FileNotFoundError("No such file exists")

        with self.assertRaises(FileNotFoundError) as context:
            run_script(["python3", "example_script.py"]) #will give error for sure
        
        # Verify the exception was raised with the expected message
        self.assertTrue("No such file exists" in str(context.exception))

    @patch('subprocess.run')
    def test_run_script_called_process_error(self, mock_subprocess_run):
        # Mock the subprocess.run to raise a CalledProcessError
        # Mimic an error like "No data.yaml/MMX059XA_COVERED5B.yaml found"
        process_mock = MagicMock()
        process_mock.returncode = 1
        process_mock.cmd = ["python3", "train.py"]
        process_mock.stdout.decode.return_value = ""
        process_mock.stderr.decode.return_value = "No yaml file found"
        mock_subprocess_run.side_effect = subprocess.CalledProcessError(
            returncode=1, cmd=process_mock.cmd, output=process_mock.stdout, stderr=process_mock.stderr
        )

        with self.assertRaises(subprocess.CalledProcessError) as context:
            run_script(["train.py", "--data", "example_data.yaml"])
        
        # Check if the error message from stderr is in the exception
        self.assertTrue("No yaml file found" in context.exception.stderr.decode())

if __name__ == '__main__':
    unittest.main()
