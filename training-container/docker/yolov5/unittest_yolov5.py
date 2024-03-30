import unittest
from unittest import mock
from train_and_export import run_script, run_script, parse_arguments, main

# import json_to_yaml_converter


class TestYOLOv5Script(unittest.TestCase):
    @mock.patch("subprocess.run")
    def test_run_script(self, mock_run):
        # Arrange:

        # Act:
        run_script(["script1.py"], use_module=False)
        run_script(["script2.py"], use_module=True)

        # Assert:
        mock_run.assert_any_call(["python3", "script1.py"], check=True)
        mock_run.assert_any_call(["python3", "-m", "script2.py"], check=True)

    @mock.patch("argparse.ArgumentParser.parse_args")
    def test_parse_arguments(self, mock_parse_args):
        # Arrange:
        scenarios = [
            {
                "img_size": "640",
                "batch": "16",
                "epochs": "10",
                "weights": "yolov5s.pt",
                "data": "data.yaml",
                "hyp": "config.yaml",
                "patience": "3",
                "workers": "4",
                "device": "0",
            },
            {
                "img_size": "1280",
                "batch": "-1",
                "epochs": "1",
                "weights": "yolov5n6.pt",
                "data": "MMX059XA_COVERED5B.yaml",
                "hyp": "advanced.yaml",
                "patience": "1",
                "workers": "1",
                "device": "1,2,3",
            },
        ]

        for scenario in scenarios:
            mock_parse_args.return_value = mock.Mock(**scenario)

            # Act:
            args = parse_arguments()

            # Assert:
            self.assertIn(args.img_size, ["640", "1280"])
            self.assertTrue(int(args.batch) == -1 or int(args.batch) > 0)
            self.assertTrue(int(args.epochs) > 0)
            self.assertTrue(
                (args.img_size == "640" and args.weights == "yolov5s.pt")
                or (args.img_size == "1280" and args.weights == "yolov5n6.pt")
            )
            self.assertIn(args.data, ["data.yaml", "MMX059XA_COVERED5B.yaml"])
            self.assertTrue(args.hyp.endswith(".yaml"))
            self.assertTrue(int(args.patience) > 0)
            self.assertTrue(int(args.workers) > 0)
            device_ids = args.device.split(",")
            for device_id in device_ids:
                self.assertIn(device_id.strip(), ["0", "1", "2", "3", "4", "5"])

    @mock.patch("train_and_export.run_script")
    @mock.patch("train_and_export.parse_arguments")
    @mock.patch("shutil.copy2")
    def test_main_workflow(self, mock_copy, mock_parse_args, mock_run_script):
        # Arrange:
        mock_parse_args.return_value = mock.Mock(
            img_size="640",
            batch="16",
            epochs="10",
            weights="yolov5n6.pt",
            data="data.yaml",
            hyp="hyp.yaml",
            project="project",
            name="test",
            patience="3",
            workers="4",
            optimizer="SGD",
            device="0,1",
            include="onnx",
        )

        # Act:
        main()

        # Assert:
        # expected_export_args = [
        #     "yolov5/export.py",
        #     "--img-size",
        #     "640",
        #     "--weights",
        #     "/opt/ml/output/data/results/weights/best.pt",
        #     "--include",
        #     "onnx",
        #     "--device",
        #     "0,1",
        # ]
        # mock_run_script.assert_any_call(expected_export_args)
        # # Verify that the best model is copied to the correct location.
        # mock_copy.assert_called_with(
        #     "/opt/ml/output/data/results/weights/best.onnx", "/opt/ml/model/"
        # )

        mock_run_script.assert_called()
        mock_copy.assert_called_with(
            "/opt/ml/output/data/results/weights/best.onnx", "/opt/ml/model/"
        )


# class TestJsonToYamlConversion(unittest.TestCase):
#     @mock.patch(
#         "builtins.open",
#         new_callable=mock.mock_open,
#         read_data='{"key1": "100", "key2": "200"}',
#     )
#     @mock.patch("argparse.ArgumentParser.parse_args")
#     @mock.patch("os.path.join", return_value="path/to/custom-hyps.yaml")
#     @mock.patch("pathlib.Path.parent", return_value="path/to")
#     @mock.patch("yaml.dump")
#     def test_conversion(
#         self, mock_yaml_dump, mock_path_join, mock_parent, mock_args, mock_file
#     ):
#         # Arrange:
#         mock_args.return_value = mock.Mock(Path="path/to/input.json")

#         # Act:
#         with mock.patch.object(json_to_yaml_converter, "__name__", "__main__"):
#             with mock.patch.object(
#                 json_to_yaml_converter, "args", mock_args.return_value
#             ):
#                 json_to_yaml_converter

#         # Assert:
#         mock_file.assert_called_once_with("path/to/input.json", "r")
#         mock_yaml_dump.assert_called_once()

#         called_args, _ = mock_yaml_dump.call_args
#         converted_data = called_args[0]

#         self.assertEqual(converted_data, {"key1": 100.0, "key2": 200.0})

#         mock_path_join.assert_called_once_with("path/to", "custom-hyps.yaml")


if __name__ == "__main__":
    unittest.main()
