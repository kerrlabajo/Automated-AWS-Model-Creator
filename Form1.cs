using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Linq;
using LSC_Trainer.Functions;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using System.Threading;

namespace LSC_Trainer
{
    public partial class Form1 : Form
    {
        private delegate void SetProgressCallback(int percentDone);
        private readonly AmazonSageMakerClient amazonSageMakerClient;
        private readonly AmazonS3Client s3Client;
        private readonly AmazonCloudWatchLogsClient cloudWatchLogsClient;

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
            cloudWatchLogsClient = new AmazonCloudWatchLogsClient(ACCESS_KEY, SECRET_KEY, region);

            string datasetName = DEFAULT_DATASET_URI.Split('/').Reverse().Skip(1).First();
            if (datasetName == "MMX059XA_COVERED5B")
            {
                txtImageSize.Text = "1280";
                txtBatchSize.Text = "1";
                txtEpochs.Text = "1";
                txtWeights.Text = "yolov5n6.pt";
                txtData.Text = "MMX059XA_COVERED5B.yaml";
                txtHyperparameters.Text = "hyp.no-augmentation.yaml";
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
                txtImageSize.Text = "640";
                txtBatchSize.Text = "1";
                txtEpochs.Text = "50";
                txtWeights.Text = "yolov5s.pt";
                txtData.Text = "data.yaml";
                txtHyperparameters.Text = "hyp.scratch-low.yaml";
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
                    isFile = false;
                    enableUploadToS3Button(true);
                }
            }
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
            InitiateTrainingJob(trainingRequest, cloudWatchLogsClient);
            btnTraining.Enabled = false;
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
            MessageBox.Show("Upload completed!");
            progressBar.Value = 0;
        }

        private void SelectAllTextOnClick(object sender, EventArgs e)
        {
            sender.GetType().GetMethod("SelectAll")?.Invoke(sender, null);
        }

        // TODO: Create another background worker for training job request.
        // TODO: Display the status of the training job request in the form.
        // TODO: Display the log stream of the training job request in the form when the training is in progress.
        // TODO: Preferably create another form when performing a new training job request.
        // The tasks should be implemented in branch `feat/training-logging`.

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

            if (txtImageSize.Text != "") img_size = txtImageSize.Text;

            if (txtBatchSize.Text != "") batch_size = txtBatchSize.Text;

            if (txtEpochs.Text != "") epochs = txtEpochs.Text;

            if (txtWeights.Text != "") weights = txtWeights.Text;

            if (txtData.Text != "") data = txtData.Text;

            if (txtHyperparameters.Text != "") hyperparameters = txtHyperparameters.Text;

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

        //private Dictionary<string, TrainingInfoForm> trainingJobs = new Dictionary<string, TrainingInfoForm>();

        private async void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest, AmazonCloudWatchLogsClient cloudWatchLogsClient)
        {
            try
            {
                CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
                string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();
                string datasetKey = customUploadsURI.Replace($"s3://{SAGEMAKER_BUCKET}/", "");
                bool deleteDataset = false;

                Console.WriteLine("Training job executed successfully.");

                DescribeTrainingJobResponse trainingDetails = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
                {
                    TrainingJobName = trainingJobName
                });

                // Create an entry for the current training job in the dictionary
                //trainingJobs[trainingJobName] = new TrainingInfoForm();
                //trainingJobs[trainingJobName].Show(); // Show the new form

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000;

                string prevStatusMessage = "";
                string prevLogMessage = "";
                int prevLogIndex = 0;
                // show panel
                panel2.Visible = true;

                timer.Tick += async (sender1, e1) =>
                {
                    try
                    {
                        DescribeTrainingJobResponse tracker = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
                        {
                            TrainingJobName = trainingJobName
                        });

                        // Update training duration
                        TimeSpan timeSpan = TimeSpan.FromSeconds(tracker.TrainingTimeInSeconds);
                        string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                        // Use the dictionary entry for the current training job
                        //var currentTrainingInfo = trainingJobs[trainingJobName];
                        //currentTrainingInfo.Text = trainingJobName;

                        if (tracker.TrainingTimeInSeconds == 0)
                        {
                            UpdateTrainingStatus(
                                tracker.ResourceConfig.InstanceType.ToString(),
                                formattedTime,
                                tracker.SecondaryStatusTransitions.Last().Status,
                                tracker.SecondaryStatusTransitions.Last().StatusMessage
                            );
                        }
                        else
                        {
                            UpdateTrainingStatus(formattedTime);
                        }

                        // CloudWatch 
                        if (tracker.SecondaryStatusTransitions.Last().Status == "Training")
                        {
                            // Get log stream
                            string logStreamName = await GetLatestLogStream(cloudWatchLogsClient, "/aws/sagemaker/TrainingJobs", trainingJobName);

                            if (!string.IsNullOrEmpty(logStreamName))
                            {
                                // Print CloudWatch logs
                                GetLogEventsResponse logs = await cloudWatchLogsClient.GetLogEventsAsync(new GetLogEventsRequest
                                {
                                    LogGroupName = "/aws/sagemaker/TrainingJobs",
                                    LogStreamName = logStreamName
                                });


                                if (prevStatusMessage != logs.Events.Last().Message)
                                {
                                    for (int i = prevLogIndex + 1; i < logs.Events.Count; i++)
                                    {
                                        DisplayLogMessage(logs.Events[i].Message);
                                    }
                                    prevStatusMessage = logs.Events.Last().Message;
                                    prevLogIndex = logs.Events.IndexOf(logs.Events.Last());
                                }
                            }
                        }
                        if (tracker.SecondaryStatusTransitions.Last().StatusMessage != prevStatusMessage)
                        {
                            UpdateTrainingStatus(
                                tracker.SecondaryStatusTransitions.Last().Status,
                                tracker.SecondaryStatusTransitions.Last().StatusMessage
                            );
                        }

                        if (tracker.TrainingJobStatus == TrainingJobStatus.Completed && deleteDataset == false)
                        {
                            DisplayLogMessage("Printing status history...");

                            foreach (SecondaryStatusTransition history in tracker.SecondaryStatusTransitions)
                            {
                                DisplayLogMessage("Status: " + history.Status);

                                TimeSpan elapsed = history.EndTime - history.StartTime;
                                string formattedElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                (int)elapsed.TotalHours,
                                                elapsed.Minutes,
                                                elapsed.Seconds,
                                                (int)(elapsed.Milliseconds / 100));
                                DisplayLogMessage($"Elapsed Time: {formattedElapsedTime}");
                                DisplayLogMessage("Description: " + history.StatusMessage + Environment.NewLine);
                            }
                            outputKey = $"training-jobs/{trainingJobName}/output/output.tar.gz";
                            modelKey = $"training-jobs/{trainingJobName}/output/model.tar.gz";
                            enableDownloadModelButton(true);
                            
                            if (HasCustomUploads(customUploadsURI) == false)
                            {
                                
                                btnTraining.Enabled = true;
                                timer.Stop();
                            }
                            else
                            {
                                deleteDataset = true;
                            }
                            
                        }

                        //delete custom dataset after training
                        if(deleteDataset)
                        {

                            DisplayLogMessage($"Deleting dataset {datasetKey} from BUCKET ${SAGEMAKER_BUCKET}");
                            AWS_Helper.DeleteDataSet(s3Client, SAGEMAKER_BUCKET, datasetKey);
                            deleteDataset = false;
                            btnTraining.Enabled = true;
                            timer.Stop();
                        }
                        
                        if (tracker.TrainingJobStatus == TrainingJobStatus.Failed)
                        {
                            Console.WriteLine(tracker.FailureReason);
                            DisplayLogMessage($"Training job failed: {tracker.FailureReason}");
                            timer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                timer.Start();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating training job: {ex.Message}");
                MessageBox.Show($"Error creating training job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnTraining.Enabled = true;
            }
        }

        public async Task<string> GetLatestLogStream(AmazonCloudWatchLogsClient amazonCloudWatchLogsClient, string logGroupName, string trainingJobName)
        {
            var request = new DescribeLogStreamsRequest
            {
                LogGroupName = logGroupName,
                LogStreamNamePrefix = trainingJobName
            };

            var response = await amazonCloudWatchLogsClient.DescribeLogStreamsAsync(request);

            var latestLogStream = response.LogStreams.FirstOrDefault();

            if (latestLogStream != null)
            {
                return latestLogStream.LogStreamName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetLatestLogStream(AmazonCloudWatchLogsClient amazonCloudWatchLogsClient, string logGroupName)
        {
            var request = new DescribeLogStreamsRequest
            {
                LogGroupName = logGroupName,
                LogStreamNamePrefix = trainingJobName
            };

            var response = await amazonCloudWatchLogsClient.DescribeLogStreamsAsync(request);

            var latestLogStream = response.LogStreams.FirstOrDefault();

            if (latestLogStream != null)
            {
                return latestLogStream.LogStreamName;
            }
            else
            {
                return null;
            }
        }

        private void newTrainingJobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Thread(() => Application.Run(new Form1()));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            instanceTypeBox.Text = instanceType;
            trainingDurationBox.Text = trainingDuration;
            trainingStatusBox.Text = status;
            descBox.Text = description;
            //PrevStatusMessage = status;
        }
        public void UpdateTrainingStatus(string trainingDuration)
        {
            trainingDurationBox.Text = trainingDuration;
        }
        public void UpdateTrainingStatus(string status, string description)
        {
            trainingStatusBox.Text = status;
            descBox.Text = description;
            //PrevStatusMessage = status;
        }

        public void DisplayLogMessage(string logMessage)
        {
            // Append log messages to the TextBox
            logBox.AppendText(logMessage + Environment.NewLine);

            // Scroll to the end to show the latest log messages
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }
    }
}
