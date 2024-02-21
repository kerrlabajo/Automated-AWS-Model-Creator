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
    public partial class MainForm : Form
    {

        private delegate void SetProgressCallback(int percentDone);
        private readonly AmazonSageMakerClient amazonSageMakerClient;
        private readonly AmazonS3Client s3Client;
        private readonly AmazonCloudWatchLogsClient cloudWatchLogsClient;
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
        private bool isValidConnectionInfo;

        private CustomHyperParamsForm customHyperParamsForm;

        public MainForm()
        {
            InitializeComponent();

            isValidConnectionInfo = !string.IsNullOrWhiteSpace(UserConnectionInfo.AccountId) &&
                                 !string.IsNullOrWhiteSpace(UserConnectionInfo.AccessKey) &&
                                 !string.IsNullOrWhiteSpace(UserConnectionInfo.SecretKey) &&
                                 !string.IsNullOrWhiteSpace(UserConnectionInfo.Region) &&
                                 !string.IsNullOrWhiteSpace(UserConnectionInfo.RoleArn);

            Console.WriteLine($"Details: {UserConnectionInfo.AccountId}{UserConnectionInfo.AccessKey}{UserConnectionInfo.SecretKey}{UserConnectionInfo.Region}{UserConnectionInfo.RoleArn}");
            if (isValidConnectionInfo)
            {
                backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.DoWork += backgroundWorker_DoWork;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
                backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

                string ENV_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .env").Replace("\\", "/");
                DotNetEnv.Env.Load(ENV_PATH);

                ACCESS_KEY = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
                SECRET_KEY = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
                REGION = Environment.GetEnvironmentVariable("REGION");
                ROLE_ARN = Environment.GetEnvironmentVariable("ROLE_ARN");

                // USING THE SINGLETON CLASS INSTEAD OF ENV FILE
                /*
                ACCESS_KEY = UserConnectionInfo.AccessKey;
                SECRET_KEY = UserConnectionInfo.SecretKey;
                REGION = UserConnectionInfo.Region;
                ROLE_ARN = UserConnectionInfo.RoleArn;
                ECR_URI = UserConnectionInfo.EcrUri;
                */
                ECR_URI = Environment.GetEnvironmentVariable("ECR_URI");
                SAGEMAKER_BUCKET = Environment.GetEnvironmentVariable("SAGEMAKER_BUCKET");
                DEFAULT_DATASET_URI = Environment.GetEnvironmentVariable("DEFAULT_DATASET_URI");
                customUploadsURI = Environment.GetEnvironmentVariable("CUSTOM_UPLOADS_URI");
                DESTINATION_URI = Environment.GetEnvironmentVariable("DESTINATION_URI");

                string selectedRegionSystemName = REGION;
                RegionEndpoint selectedRegionEndpoint = RegionEndpoint.GetBySystemName(selectedRegionSystemName);
                var awsCredentials = new BasicAWSCredentials(ACCESS_KEY, SECRET_KEY);

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
            btnUploadToS3.Enabled = false;
            btnDownloadModel.Enabled = false;
        }
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
                    btnUploadToS3.Enabled = true;
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
                    btnUploadToS3.Enabled = true;
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
            logBox.Clear();
            instanceTypeBox.Text = "";
            trainingDurationBox.Text = "";
            trainingStatusBox.Text = "";
            descBox.Text = "";
            Cursor = Cursors.WaitCursor;
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

            if (HasCustomUploads(customUploadsURI))
            {
                InitiateTrainingJob(trainingRequest, cloudWatchLogsClient);
            }
            else
            {
                DialogResult result = MessageBox.Show("No custom dataset uploaded. The default dataset will be used for training instead. Do you want to proceed?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    InitiateTrainingJob(trainingRequest, cloudWatchLogsClient);
                }
                else
                {
                    return;
                }
            }
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
                        try
                        {
                            Cursor = Cursors.WaitCursor;
                            logBox.Visible = true;
                            logBox.Cursor = Cursors.WaitCursor;
                            mainPanel.Enabled = false;
                            string outputResponse = await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, outputKey, selectedLocalPath);
                            DisplayLogMessage(outputResponse);
                            string modelResponse = await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, modelKey, selectedLocalPath);
                            DisplayLogMessage(modelResponse);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Error downloading model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                            logBox.Cursor = Cursors.Default;
                            mainPanel.Enabled = true;
                        }
                        
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
                HyperParameters = customHyperParamsForm.HyperParameters,
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

        private void InputsEnabler(bool intent)
        {
            imgSizeDropdown.Enabled = intent;
            txtBatchSize.Enabled = intent;
            txtEpochs.Enabled = intent;
            txtWeights.Enabled = intent;
            txtData.Enabled = intent;
            hyperparamsDropdown.Enabled = intent;
            txtPatience.Enabled = intent;
            txtWorkers.Enabled = intent;
            txtOptimizer.Enabled = intent;
            txtDevice.Enabled = intent;
            btnSelectDataset.Enabled = intent;
            btnSelectFolder.Enabled = intent;
            btnUploadToS3.Enabled = intent;
            btnTraining.Enabled = intent;
            modelListComboBox.Enabled = intent;
            btnFetchModels.Enabled = intent;
            btnDownloadModel.Enabled = intent;
            lblZipFile.Enabled = intent;
            logBox.UseWaitCursor = !intent;
        }
        private async void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest, AmazonCloudWatchLogsClient cloudWatchLogsClient)
        {
            InputsEnabler(false);
            Cursor = Cursors.WaitCursor;
            try
            {
                CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
                string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();
                string datasetKey = customUploadsURI.Replace($"s3://{SAGEMAKER_BUCKET}/", "");

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
                logPanel.Visible = true;

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

                        if (tracker.TrainingJobStatus == TrainingJobStatus.Completed)
                        {
                            InputsEnabler(true);
                            Cursor = Cursors.Default;
                            outputKey = $"training-jobs/{trainingJobName}/output/output.tar.gz";
                            modelKey = $"training-jobs/{trainingJobName}/output/model.tar.gz";
                            timer.Stop();

                            if (HasCustomUploads(customUploadsURI))
                            {
                                DisplayLogMessage($"{Environment.NewLine}Deleting dataset {datasetKey} from BUCKET ${SAGEMAKER_BUCKET}");
                                AWS_Helper.DeleteDataSet(s3Client, SAGEMAKER_BUCKET, datasetKey);
                            }
                            return;
                        }
                        else if(tracker.TrainingJobStatus == TrainingJobStatus.Failed)
                        {
                            DisplayLogMessage($"Training job failed: {tracker.FailureReason}");
                            btnTraining.Enabled = true;
                            timer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating training job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnTraining.Enabled = true;
                return;
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

        private void newTrainingJobMenu_Click(object sender, EventArgs e)
        {
            var t = new Thread(() => Application.Run(new MainForm()));
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

                customHyperParamsForm = new CustomHyperParamsForm();

                customHyperParamsForm.FormClosed += OtherForm_FormClosed;
                customHyperParamsForm.Show();
            }
            else
            {
                hyperparamsDropdown.Text = hyperparamsDropdown.GetItemText(hyperparamsDropdown.SelectedItem);
            }
        }

        private void helpMenu_Click(object sender, EventArgs e)
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

        private void testConnnectionToolStripMenuItem_Click(object sender, EventArgs e)
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

        private async void btnFetchModels_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            mainPanel.Enabled = false;
            try
            {
                List<string> models = await AWS_Helper.GetModelListFromS3(s3Client, SAGEMAKER_BUCKET);

                if (models != null)
                {
                    modelListComboBox.Items.Clear(); 

                    foreach (var obj in models)
                    {
                        modelListComboBox.Items.Add(obj); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                mainPanel.Enabled = true;
                modelListComboBox.Enabled = true;
            }
        }

        private void modelListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (modelListComboBox.GetItemText(hyperparamsDropdown.SelectedItem) != null)
            {
                outputKey = modelListComboBox.GetItemText(modelListComboBox.SelectedItem);
                btnDownloadModel.Enabled = true;
            }
        }

        private void testConnectionToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void createConnectionMenu_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var createConnectionForm = new CreateConnectionForm();
            createConnectionForm.FormClosed += OtherForm_FormClosed;
            createConnectionForm.Show();
        }

        public string GetRoleDetailsAsync(string roleArn)
        {
            try
            {
                /*var response = await _iamClient.GetRoleAsync(new GetRoleRequest
                {
                    RoleName = ExtractRoleNameFromArn(roleArn)
                });

                bool isAdmin = await IsAdminRole(response.Role);

                string pattern = @"^arn:aws:iam::(\d{12}):role\/([a-zA-Z0-9_\+=,@\.-]+)$";
                Regex regex = new Regex(pattern);*/

                bool isAdmin = roleArn.Contains(":role/admin");

                return isAdmin ? "admin" : "employee";
            }
            /*catch (AmazonServiceException error)
            {
                Console.WriteLine($"AWS Service Error: {error.Message}");
                Console.WriteLine($"StatusCode: {error.StatusCode}, ErrorCode: {error.ErrorCode}, RequestId: {error.RequestId}");
            }*/
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving role: {ex.Message}");
            }

            return null;
        }

        private static string ExtractRoleNameFromArn(string roleArn)
        {
            var splitArn = roleArn.Split(':');
            var roleName = splitArn.Last().Split('/').Last();
            return roleName;
        }

        private async Task<bool> IsAdminRole(Role role)
        {
            bool isAdmin = false;

            var managedPoliciesResponse = await _iamClient.ListAttachedRolePoliciesAsync(new ListAttachedRolePoliciesRequest
            {
                RoleName = role.RoleName
            });

            isAdmin |= managedPoliciesResponse.AttachedPolicies.Any(policy => policy.PolicyName == "AdministratorAccess");

            return isAdmin;
        }

        private void btnBuildImage_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            var imageBuilderForm = new ImageBuilderForm();
            imageBuilderForm.FormClosed += OtherForm_FormClosed;
            imageBuilderForm.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
           
            Form form = new MainForm();
            form.Refresh();
            btnBuildImage.Enabled = true;
        }

        private void closeConnectionMenu_Click(object sender, EventArgs e)
        {
            UserConnectionInfo.Instance.Reset();
        }
    }
}
