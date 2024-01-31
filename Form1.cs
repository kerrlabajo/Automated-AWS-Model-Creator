using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using System.Linq;
using LSC_Trainer.Functions;

namespace LSC_Trainer
{
    public partial class Form1 : Form
    {

        private delegate void SetProgressCallback(int percentDone);
        private readonly AmazonSageMakerClient amazonSageMakerClient;
        private readonly AmazonS3Client s3Client;
        private Utility utility = new Utility();

        private readonly string ACCESS_KEY;
        private readonly string SECRET_KEY;
        private readonly string REGION;
        private readonly string ROLE_ARN;

        private readonly string ECR_URI;
        private readonly string SAGEMAKER_BUCKET;
        private readonly string DEFAULT_DATASET_URI;
        private readonly string DESTINATION_URI;

        private readonly string SAGEMAKER_INPUT_DATA_PATH = "/opt/ml/input/data/";
        private readonly string SAGEMAKER_OUTPUT_DATA_PATH = "/opt/ml/output/data/";
        private readonly string SAGEMAKER_MODEL_PATH = "/opt/ml/model/";

        private string datasetPath;
        private bool isFile;
        private string folderOrFileName;
        private string customUploadsURI;

        private string trainingFolder;
        private string validationFolder;
        
        private string trainingJobName;

        private string outputKey;
        private string modelKey;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            string ENV_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .env").Replace("\\","/");
            DotNetEnv.Env.Load(ENV_PATH);
            
            ACCESS_KEY = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
            SECRET_KEY = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
            REGION = Environment.GetEnvironmentVariable("REGION");
            ROLE_ARN = Environment.GetEnvironmentVariable("ROLE_ARN");

            ECR_URI = Environment.GetEnvironmentVariable("ECR_URI");
            SAGEMAKER_BUCKET = Environment.GetEnvironmentVariable("SAGEMAKER_BUCKET");
            DEFAULT_DATASET_URI = Environment.GetEnvironmentVariable("DEFAULT_DATASET_URI");
            customUploadsURI = Environment.GetEnvironmentVariable("CUSTOM_UPLOADS_URI");
            DESTINATION_URI = Environment.GetEnvironmentVariable("DESTINATION_URI");

            RegionEndpoint region = RegionEndpoint.GetBySystemName(REGION);
            amazonSageMakerClient = new AmazonSageMakerClient(ACCESS_KEY, SECRET_KEY, region);
            s3Client = new AmazonS3Client(ACCESS_KEY, SECRET_KEY, region);

            string datasetName = DEFAULT_DATASET_URI.Split('/').Reverse().Skip(1).First();
            if (datasetName == "MMX059XA_COVERED5B")
            {
                imgSizeDropdown.Text = "1280";
                txtBatchSize.Text = "1";
                txtEpochs.Text = "1";
                txtWeights.Text = "yolov5n6.pt";
                txtData.Text = "MMX059XA_COVERED5B.yaml";
                hyperparamsDropdown.Text = "hyp.no-augmentation.yaml";
                txtPatience.Text = "100";
                txtWorkers.Text = "8";
                txtOptimizer.Text = "SGD";
                txtDevice.Text = "cpu";
                // txtDevice.Text = "0";
                trainingFolder = "train";
                validationFolder = "Verification Images";
            }
            else
            {
                imgSizeDropdown.Text = "640";
                txtBatchSize.Text = "1";
                txtEpochs.Text = "50";
                txtWeights.Text = "yolov5s.pt";
                txtData.Text = "data.yaml";
                hyperparamsDropdown.Text = "hyp.scratch-low.yaml";
                txtPatience.Text = "100";
                txtWorkers.Text = "8";
                txtOptimizer.Text = "SGD";
                txtDevice.Text = "cpu";
                // txtDevice.Text = "0";
                trainingFolder = "train";
                validationFolder = "val";
            }
            enableUploadToS3Button(false);
            enableDownloadModelButton(false);
        }

        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var response = amazonSageMakerClient.ListModelsAsync(new ListModelsRequest()).Result;
                Console.WriteLine("Connection successful.");
                MessageBox.Show("Connection successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Unexpected error: {error.Message}");
                MessageBox.Show($"Connection failed: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TODO: Update training parameters:
        // - img_size (dropdown)
        // - weights (very dependent of img_size)(not interactable but changeable)
        // - hyperparameters (dropdown)(has button to customize specific params)
        // Customizing hyperparameters will open a new form that contains the configurable parameters
        // via meter/toggle bars that shows or manually edit its fractional values.
        // These values will then be entered in `HyperParameters` in the `CreateTrainingJobRequest`.
        // (This is still not guaranteed but this requires writing the values the same format as the `hyp.scratch.yaml` file
        // from the yolov5 repository in the user's directory/memory stream.)
        // (Do not implement this feature yet. This is only for consideration. Keep it as is.)
        // TODO: Add logout.
        // TODO: Add help link.


        // TODO: Download model should first retrieve list of models from S3 bucket that was fetched from 
        // one or more training jobs. The user will then select the model to download.
        // This features guarantees that the form can be closed and reopened without losing the model.
        // TODO: Display the log stream of the training job request in the form when the training is in progress.
        // TODO: Preferably create another form when performing a new training job request.
        // The tasks should be implemented in branch `feat/ui-qol-fix`.

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void enableUploadToS3Button(bool intent)
        {
            btnUploadToS3.Enabled = intent;
        }

        private void enableDownloadModelButton(bool intent)
        {
            btnDownloadModel.Enabled = intent;
        }

        private void btnSelectDataset_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ZIP Files (*.zip)|*.zip|RAR Files (*.rar)|*.rar";
                openFileDialog.Title = "Select a Zip or Rar File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    datasetPath = openFileDialog.FileName;

                    // Display the selected file path (optional)
                    lblZipFile.Text = datasetPath;

                    MessageBox.Show($"Selected file: {datasetPath}");
                    btnRemoveFile.Visible = true;
                    isFile = true;
                    enableUploadToS3Button(true);
                }
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder";
                folderBrowserDialog.ShowNewFolderButton = false; // Optional: Set to true if you want to allow the user to create a new folder

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    datasetPath = folderBrowserDialog.SelectedPath;

                    // Display the selected folder path (optional)
                    lblZipFile.Text = datasetPath;

                    MessageBox.Show($"Selected folder: {datasetPath}");
                    btnRemoveFile.Visible = true;
                    isFile = false;
                    enableUploadToS3Button(true);
                }
            }
        }
        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            datasetPath = null;
            lblZipFile.Text = "No file selected";
            btnRemoveFile.Visible = false;
            enableUploadToS3Button(false);
        }

        private void btnUploadToS3_Click(object sender, EventArgs e)
        {
            if(datasetPath != null)
            {
                folderOrFileName = datasetPath.Split('\\').Last();
                DialogResult result = MessageBox.Show($"Do you want to upload {folderOrFileName} to s3 bucket?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) backgroundWorker.RunWorkerAsync();

                // For testing purposes. Pre-define values.
                trainingFolder = "train";
                validationFolder = "val";
                }
            else
            {
                MessageBox.Show("No file to upload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTraining_Click(object sender, EventArgs e)
        {
            SetTrainingParameters(
                out string img_size,
                out string batch_size,
                out string epochs,
                out string weights,
                out string data,
                out string hyperparameters,
                out string patience,
                out string workers,
                out string optimizer,
                out string device);

            trainingJobName = string.Format("Ubuntu-CUDA-YOLOv5-Training-{0}", DateTime.Now.ToString("yyyy-MM-dd-hh-mmss"));
            CreateTrainingJobRequest trainingRequest = CreateTrainingRequest(
                img_size, batch_size, epochs, weights, data, hyperparameters, patience, workers, optimizer, device);
            InitiateTrainingJob(trainingRequest);
        }

        private async void btnDownloadModel_Click(object sender, EventArgs e)
        {
            //string temporaryOutputKey = "training-jobs/Ubuntu-CUDA-YOLOv5-Training-2024-01-30-06-0039/output/output.tar.gz";
            //string temporaryModelKey = "training-jobs/Ubuntu-CUDA-YOLOv5-Training-2024-01-30-06-0039/output/model.tar.gz";

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder to save the results and model";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedLocalPath = folderBrowserDialog.SelectedPath;

                    DialogResult result = MessageBox.Show($"Do you want to save the results and model to {selectedLocalPath} ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, outputKey, selectedLocalPath);
                        await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, modelKey, selectedLocalPath);
                    }
                }
            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (isFile)
            {
                AWS_Helper.UnzipAndUploadToS3(s3Client, SAGEMAKER_BUCKET, datasetPath, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
                customUploadsURI = customUploadsURI + Path.GetFileNameWithoutExtension(datasetPath) + "/";
            }
            else
            {
                AWS_Helper.UploadFolderToS3(s3Client, datasetPath, "custom-uploads/" + folderOrFileName, SAGEMAKER_BUCKET, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
                customUploadsURI = customUploadsURI + folderOrFileName + "/";
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= progressBar.Minimum && e.ProgressPercentage <= progressBar.Maximum)
            {
            progressBar.Value = e.ProgressPercentage;
        }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            btnRemoveFile_Click(sender, e);
            MessageBox.Show("Upload completed!");
            progressBar.Value = 0;
        }

        private void SelectAllTextOnClick(object sender, EventArgs e)
        {
            sender.GetType().GetMethod("SelectAll")?.Invoke(sender, null);
        }

        private void SetTrainingParameters(out string img_size, out string batch_size, out string epochs, out string weights, out string data, out string hyperparameters, out string patience, out string workers, out string optimizer, out string device)
        {
            img_size = "";
            batch_size = "";
            epochs = "";
            weights = "";
            data = "";
            hyperparameters = "";
            patience = "";
            workers = "";
            optimizer = "";
            device = "";

            if (imgSizeDropdown.Text != "") img_size = imgSizeDropdown.Text;

            if (txtBatchSize.Text != "") batch_size = txtBatchSize.Text;

            if (txtEpochs.Text != "") epochs = txtEpochs.Text;

            if (txtWeights.Text != "") weights = txtWeights.Text;

            if (txtData.Text != "") data = txtData.Text;

            if (hyperparamsDropdown.Text != "") hyperparameters = hyperparamsDropdown.Text;

            if (txtPatience.Text != "") patience = txtPatience.Text;

            if (txtWorkers.Text != "") workers = txtWorkers.Text;

            if (txtOptimizer.Text != "") optimizer = txtOptimizer.Text;

            if (txtDevice.Text != "") device = txtDevice.Text;
        }

        private static bool HasCustomUploads(string customUploadsURI)
        {
            string customUploadsDirectory = Path.GetFileName(Path.GetDirectoryName(customUploadsURI));
            if (customUploadsDirectory == "custom-uploads")
            {
                return false;
            }
            return true;
        }

        private CreateTrainingJobRequest CreateTrainingRequest(string img_size, string batch_size, string epochs, string weights, string data, string hyperparameters, string patience, string workers, string optimizer, string device)
        {
            if (Path.GetFileName(customUploadsURI) == "custom-uploads")
            {
                Console.WriteLine(customUploadsURI + "failed");
                MessageBox.Show("Please upload a dataset first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("Please upload a dataset first.");
            }

            CreateTrainingJobRequest trainingRequest = new CreateTrainingJobRequest()
            {
                AlgorithmSpecification = new AlgorithmSpecification()
                {
                    TrainingInputMode = "File",
                    TrainingImage = ECR_URI,
                    ContainerEntrypoint = new List<string>() { "python3", "yolov5/train_and_export.py" },
                    ContainerArguments = new List<string>()
                    {
                        "--img-size", img_size,
                        "--batch", batch_size,
                        "--epochs", epochs,
                        "--weights", weights,
                        "--data", SAGEMAKER_INPUT_DATA_PATH + "train/" + data,
                        "--hyp", hyperparameters,
                        "--project", SAGEMAKER_OUTPUT_DATA_PATH,
                        "--name", "results",
                        "--patience", patience,
                        "--workers", workers,
                        "--optimizer", optimizer,
                        "--device", device,
                        "--img-size", img_size,
                        "--weights", SAGEMAKER_OUTPUT_DATA_PATH + "results/weights/best.pt",
                        "--include", "onnx",
                        "--device", device
                    }
                },
                RoleArn = ROLE_ARN,
                OutputDataConfig = new OutputDataConfig()
                {
                    S3OutputPath = DESTINATION_URI
                },
                ResourceConfig = new ResourceConfig()
                {
                    InstanceCount = 1,
                    InstanceType = TrainingInstanceType.MlM5Xlarge,
                    VolumeSizeInGB = 12
                },
                TrainingJobName = trainingJobName,
                StoppingCondition = new StoppingCondition()
                {
                    MaxRuntimeInSeconds = 360000
                },
                InputDataConfig = new List<Channel>(){
                    new Channel()
                    {
                        ChannelName = "train",
                        InputMode = TrainingInputMode.File,
                        CompressionType = Amazon.SageMaker.CompressionType.None,
                        RecordWrapperType = RecordWrapper.None,
                        DataSource = new DataSource()
                        {
                            S3DataSource = new S3DataSource()
                            {
                                S3DataType = S3DataType.S3Prefix,
                                S3Uri = (HasCustomUploads(customUploadsURI) ? customUploadsURI : DEFAULT_DATASET_URI) + trainingFolder,
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    },
                    new Channel()
                    {
                        ChannelName = "val",
                        InputMode = TrainingInputMode.File,
                        CompressionType = Amazon.SageMaker.CompressionType.None,
                        RecordWrapperType = RecordWrapper.None,
                        DataSource = new DataSource()
                        {
                            S3DataSource = new S3DataSource()
                            {
                                S3DataType = S3DataType.S3Prefix,
                                S3Uri = (HasCustomUploads(customUploadsURI) ? customUploadsURI : DEFAULT_DATASET_URI) + validationFolder,
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    }
                }
            };
            return trainingRequest;
        }

        private void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest)
        {
            try
            {
                CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
                string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();

                Console.WriteLine("Training job executed successfully.");

                string prevStatusMessage = "";
                Timer timer = new Timer();
                timer.Interval = 5000;
                timer.Tick += async (sender1, e1) =>
                {
                    try
                    {
                        DescribeTrainingJobResponse tracker = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
                        {
                            TrainingJobName = trainingJobName
                        });

                        if (tracker.SecondaryStatusTransitions.Last().StatusMessage != prevStatusMessage)
                        {
                            Console.WriteLine($"Status: {tracker.SecondaryStatusTransitions.Last().Status}");
                            Console.WriteLine($"Description: {tracker.SecondaryStatusTransitions.Last().StatusMessage}");
                            Console.WriteLine();
                            prevStatusMessage = tracker.SecondaryStatusTransitions.Last().StatusMessage;
                        }

                        if (tracker.TrainingJobStatus == TrainingJobStatus.Completed)
                        {
                            Console.WriteLine("Printing status history...");
                            foreach (SecondaryStatusTransition history in tracker.SecondaryStatusTransitions)
                            {
                                Console.WriteLine("Status: " + history.Status);
                                TimeSpan elapsed = history.EndTime - history.StartTime;
                                string formattedElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                              (int)elapsed.TotalHours,
                                              elapsed.Minutes,
                                              elapsed.Seconds,
                                              (int)(elapsed.Milliseconds / 100));
                                Console.WriteLine($"Elapsed Time: {formattedElapsedTime}");
                                Console.WriteLine("Description: " + history.StatusMessage);
                                Console.WriteLine();
                            }
                            outputKey = $"training-jobs/{trainingJobName}/output/output.tar.gz";
                            modelKey = $"training-jobs/{trainingJobName}/output/model.tar.gz";
                            enableDownloadModelButton(true);
                            timer.Stop();
                        }
                        if (tracker.TrainingJobStatus == TrainingJobStatus.Failed)
                        {
                            Console.WriteLine(tracker.FailureReason);
                            timer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in training model: {ex.Message}");
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating training job: {ex.Message}");
            }
        }

        private void imgSizeDropdown_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedSize = imgSizeDropdown.GetItemText(imgSizeDropdown.SelectedItem);
            string weightFile = utility.GetWeightFile(selectedSize);

            if (weightFile != null)
            {
                txtWeights.Text = weightFile;
            }
            else
            {
                // Default value in the case where the size is not found
                txtWeights.Text = "640";
            }
        }

        private void hyperparamsDropdown_SelectedValueChanged(object sender, EventArgs e)
        {
            if(hyperparamsDropdown.GetItemText(hyperparamsDropdown.SelectedItem).ToLower() == "custom")
            {
                this.Enabled = false;

                var customHyperparameterForm = new CustomHyperParamsForm();

                customHyperparameterForm.FormClosed += OtherForm_FormClosed;
                customHyperparameterForm.Show();
            }
            else
            {
                hyperparamsDropdown.Text = hyperparamsDropdown.GetItemText(hyperparamsDropdown.SelectedItem);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var helpForm = new HelpForm();

            helpForm.FormClosed += OtherForm_FormClosed;
            helpForm.Show();
        }

        private void OtherForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
        }
    }
}
