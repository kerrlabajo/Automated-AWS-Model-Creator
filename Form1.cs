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
using System.Timers;

namespace LSC_Trainer
{
    public partial class MainForm : Form
    {

        private delegate void SetProgressCallback(int percentDone);
        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonS3Client s3Client;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;
        private Utility utility = new Utility();

        private string ACCOUNT_ID;
        private string ACCESS_KEY;
        private string SECRET_KEY;
        private string REGION;
        private string ROLE_ARN;

        private string ECR_URI;
        private string SAGEMAKER_BUCKET;
        private string DEFAULT_DATASET_URI;
        private string CUSTOM_UPLOADS_URI;
        private readonly string DESTINATION_URI;

        private readonly string SAGEMAKER_INPUT_DATA_PATH = "/opt/ml/input/data/";
        private readonly string SAGEMAKER_OUTPUT_DATA_PATH = "/opt/ml/output/data/";
        private readonly string SAGEMAKER_MODEL_PATH = "/opt/ml/model/";

        private string datasetPath;
        private bool isFile;
        private string folderOrFileName;

        private string trainingFolder;
        private string validationFolder;
        
        private string trainingJobName;

        private string outputKey;
        private string modelKey;

        public bool development;

        private string selectedInstance;
        private CustomHyperParamsForm customHyperParamsForm;

        //TODO: 1. Refactor all variables, methods, and classes to use the same naming convention.
        //TODO: 2. Refactor repetitive code to use methods.
        //TODO: 3. Refactor to use async/await for all methods that are not async.
        //TODO: 4. Refactor to transfer methods in their respective classes/libraries.
        //TODO: 5. Clean up code.
        //TODO: 6. The app should still work after refactoring.
        //TODO: 7. The order of function/method calls should not change after refactoring.

        public MainForm(bool development)
        {
            InitializeComponent();
            this.development = development;

            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            SAGEMAKER_BUCKET = Environment.GetEnvironmentVariable("SAGEMAKER_BUCKET");
            DEFAULT_DATASET_URI = Environment.GetEnvironmentVariable("DEFAULT_DATASET_URI");
            CUSTOM_UPLOADS_URI = Environment.GetEnvironmentVariable("CUSTOM_UPLOADS_URI");
            DESTINATION_URI = Environment.GetEnvironmentVariable("DESTINATION_URI");
            REGION = Environment.GetEnvironmentVariable("DEFAULT_REGION");
            ROLE_ARN = Environment.GetEnvironmentVariable("DEFAULT_ROLE_ARN");
            InitializeInputs();

            if (development)
            {
                string ENV_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .env").Replace("\\", "/");
                DotNetEnv.Env.Load(ENV_PATH);

                UserConnectionInfo.AccountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
                UserConnectionInfo.AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
                UserConnectionInfo.SecretKey = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
                UserConnectionInfo.Region = Environment.GetEnvironmentVariable("REGION");
                UserConnectionInfo.RoleArn = Environment.GetEnvironmentVariable("ROLE_ARN");
                UserConnectionInfo.SagemakerBucket = Environment.GetEnvironmentVariable("SAGEMAKER_BUCKET");
                MessageBox.Show("Established Connection using ENV for Development", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InitializeClient();
            }
            else
            {
                var createConnectionForm = new CreateConnectionForm(development, this);
                createConnectionForm.FormClosed += OtherForm_FormClosed;
                createConnectionForm.Show();
                this.Enabled = false;
                this.TopLevel = false;
                Console.WriteLine($"Establishing Connection...");
            }
            
            logBox.Rtf = @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}";
            btnTraining.Enabled = false;
            btnUploadToS3.Enabled = false;
            btnDownloadModel.Enabled = false;
        }

        public void InitializeClient()
        {
            ACCOUNT_ID = UserConnectionInfo.AccountId;
            ACCESS_KEY = UserConnectionInfo.AccessKey;
            SECRET_KEY = UserConnectionInfo.SecretKey;
            REGION = UserConnectionInfo.Region;
            ROLE_ARN = UserConnectionInfo.RoleArn;
            ECR_URI = GetECRUri();
            SAGEMAKER_BUCKET = UserConnectionInfo.SagemakerBucket;
            RegionEndpoint region = RegionEndpoint.GetBySystemName(REGION);
            amazonSageMakerClient = new AmazonSageMakerClient(ACCESS_KEY, SECRET_KEY, region);
            s3Client = new AmazonS3Client(ACCESS_KEY, SECRET_KEY, region);
            cloudWatchLogsClient = new AmazonCloudWatchLogsClient(ACCESS_KEY, SECRET_KEY, region);

            Console.WriteLine($"ACCOUNT_ID: {ACCOUNT_ID}");
            Console.WriteLine($"ACCESS_KEY: {ACCESS_KEY}");
            Console.WriteLine($"SECRET_KEY: {SECRET_KEY}");
            Console.WriteLine($"REGION: {REGION}");
            Console.WriteLine($"ROLE_ARN: {ROLE_ARN}");
            Console.WriteLine($"ECR_URI: {ECR_URI}");
            Console.WriteLine($"SAGEMAKER_BUCKET: {SAGEMAKER_BUCKET}");
            Console.WriteLine($"DEFAULT_DATASET_URI: {DEFAULT_DATASET_URI}");
            Console.WriteLine($"DESTINATION_URI: {DESTINATION_URI}");
        }

        public void InitializeInputs()
        {
            string datasetName = DEFAULT_DATASET_URI.Split('/').Reverse().Skip(1).First();
            if (datasetName == "MMX059XA_COVERED5B")
            {
                imgSizeDropdown.Text = "1280";
                txtBatchSize.Text = "16";
                txtEpochs.Text = "1";
                txtWeights.Text = "yolov5n6.pt";
                txtData.Text = "MMX059XA_COVERED5B.yaml";
                hyperparamsDropdown.Text = "hyp.no-augmentation.yaml";
                txtPatience.Text = "100";
                txtWorkers.Text = "8";
                txtOptimizer.Text = "SGD";
                txtDevice.Text = "0";
                trainingFolder = "train";
                validationFolder = "val";
            }
            else
            {
                imgSizeDropdown.Text = "640";
                txtBatchSize.Text = "1";
                txtEpochs.Text = "1";
                txtWeights.Text = "yolov5s.pt";
                txtData.Text = "data.yaml";
                hyperparamsDropdown.Text = "hyp.no-augmentation.yaml";
                txtPatience.Text = "100";
                txtWorkers.Text = "8";
                txtOptimizer.Text = "SGD";
                txtDevice.Text = "cpu";
                trainingFolder = "train";
                validationFolder = "val";
            }
        }

        public string GetECRUri()
        {
            return AWS_Helper.GetFirstRepositoryUri(ACCESS_KEY, SECRET_KEY, RegionEndpoint.GetBySystemName(REGION));
        }

        private void btnSelectZip_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ZIP Files (*.zip)|*.zip";
                openFileDialog.Title = "Select a Zip File";

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
                folderBrowserDialog.ShowNewFolderButton = false;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    datasetPath = folderBrowserDialog.SelectedPath;

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
                mainPanel.Enabled = false;
                logPanel.Enabled = false;
                connectionMenu.Enabled = false;
                Cursor = Cursors.WaitCursor;
                lscTrainerMenuStrip.Cursor = Cursors.Default;
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

            string modifiedInstance = selectedInstance.ToUpper().Replace(".", "").Replace("ML", "").Replace("XLARGE", "XL");
            trainingJobName = string.Format("LSCI-{0}-TRNG-IMGv6-4-{1}", modifiedInstance, DateTime.Now.ToString("yyyy-MM-dd-HH-mmss"));
            CreateTrainingJobRequest trainingRequest = CreateTrainingRequest(
                img_size, batch_size, epochs, weights, data, hyperparameters, patience, workers, optimizer, device);

            if (HasCustomUploads(CUSTOM_UPLOADS_URI))
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
                            mainPanel.Enabled = false;
                            logPanel.Enabled = false;
                            connectionMenu.Enabled = false;
                            Cursor = Cursors.WaitCursor;
                            lscTrainerMenuStrip.Cursor = Cursors.Default;
                            string outputResponse = await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, outputKey, selectedLocalPath);
                            TrainingJobHandler.DisplayLogMessage(outputResponse, logBox);
                            string modelResponse = await AWS_Helper.DownloadObjects(s3Client, SAGEMAKER_BUCKET, modelKey, selectedLocalPath);
                            TrainingJobHandler.DisplayLogMessage(modelResponse, logBox);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Error downloading model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            mainPanel.Enabled = true;
                            logPanel.Enabled = true;
                            connectionMenu.Enabled = true;
                            Cursor = Cursors.Default;
                            System.Diagnostics.Process.Start(selectedLocalPath);
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
                CUSTOM_UPLOADS_URI = CUSTOM_UPLOADS_URI + Path.GetFileNameWithoutExtension(datasetPath) + "/";
            }
            else
            {
                AWS_Helper.UploadFolderToS3(s3Client, datasetPath, "custom-uploads/" + folderOrFileName, SAGEMAKER_BUCKET, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
                CUSTOM_UPLOADS_URI = CUSTOM_UPLOADS_URI + folderOrFileName + "/";
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
            mainPanel.Enabled = true;
            logPanel.Enabled = true;
            connectionMenu.Enabled = true;
            Cursor = Cursors.Default;
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
                        "--include", "onnx",
                    }
                },
                RoleArn = ROLE_ARN,
                OutputDataConfig = new OutputDataConfig()
                {
                    S3OutputPath = DESTINATION_URI
                },
                // Keep this true so that devs will be using spot instances from now on
                EnableManagedSpotTraining = true,
                ResourceConfig = new ResourceConfig()
                {
                    InstanceCount = 1,
                    // Update the instance type everytime you select an instance type
                    InstanceType = TrainingInstanceType.FindValue(selectedInstance),
                    VolumeSizeInGB = 12
                },
                TrainingJobName = trainingJobName,
                StoppingCondition = new StoppingCondition()
                {
                    MaxRuntimeInSeconds = 14400,
                    MaxWaitTimeInSeconds = 15000,
                },
                HyperParameters = hyperparameters != "Custom" ? new Dictionary<string, string>()
                {
                    {"img-size", img_size},
                    {"batch-size", batch_size},
                    {"epochs", epochs},
                    {"weights", weights},
                    {"hyp", hyperparameters},
                    {"patience", patience},
                    {"workers", workers},
                    {"optimizer", optimizer},
                    {"device", device},
                    {"include", "onnx" }
                }
                : customHyperParamsForm.HyperParameters,
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
                                S3Uri = (HasCustomUploads(CUSTOM_UPLOADS_URI) ? CUSTOM_UPLOADS_URI : DEFAULT_DATASET_URI) + trainingFolder,
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
                                S3Uri = (HasCustomUploads(CUSTOM_UPLOADS_URI) ? CUSTOM_UPLOADS_URI : DEFAULT_DATASET_URI) + validationFolder,
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    }
                }
            };
            return trainingRequest;
        }

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
            outputListComboBox.Enabled = intent;
            instancesDropdown.Enabled = intent;
            btnFetchOutput.Enabled = intent;
            btnDownloadModel.Enabled = intent;
            lblZipFile.Enabled = intent;
            logBox.UseWaitCursor = !intent;
        }
        private async void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest, AmazonCloudWatchLogsClient cloudWatchLogsClient)
        {
            InputsEnabler(false);
            connectionMenu.Enabled = false;
            logPanel.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
            this.Text = trainingJobName;
            try
            {
                CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
                string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();
                string datasetKey = CUSTOM_UPLOADS_URI.Replace($"s3://{SAGEMAKER_BUCKET}/", "");

                logPanel.Visible = true;
                var handler = new TrainingJobHandler(amazonSageMakerClient, cloudWatchLogsClient, s3Client,instanceTypeBox, trainingDurationBox, trainingStatusBox, descBox, logBox);
                bool custom = HasCustomUploads(CUSTOM_UPLOADS_URI);
                bool success =  await handler.StartTrackingTrainingJob(trainingJobName, datasetKey, SAGEMAKER_BUCKET, custom);
                
                if (success)
                { 
                    outputKey = $"training-jobs/{trainingJobName}/output/output.tar.gz";
                    modelKey = $"training-jobs/{trainingJobName}/output/model.tar.gz";
                }

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating training job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnTraining.Enabled = true;
                return;
            }
            finally
            {
                InputsEnabler(true);
                connectionMenu.Enabled = true;
                logPanel.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
        private void newTrainingJobMenu_Click(object sender, EventArgs e)
        {
            var t = new Thread(() => Application.Run(new MainForm(development)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void DisplayLogMessage(string logMessage)
        {
            // Convert the ANSI log message to RTF
            string rtfMessage = ConvertAnsiToRtf(logMessage);

            // Remove the start and end of the RTF document from the message
            rtfMessage = rtfMessage.Substring(rtfMessage.IndexOf('}') + 1);
            rtfMessage = rtfMessage.Substring(0, rtfMessage.LastIndexOf('}'));

            // Append the RTF message at the end of the existing RTF text
            logBox.Rtf = logBox.Rtf.Insert(logBox.Rtf.LastIndexOf('}'), rtfMessage);

            // Scroll to the end to show the latest log messages
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        public string ConvertAnsiToRtf(string ansiText)
        {
            ansiText = ansiText.Replace("#033[1m", @"\b ");
            ansiText = ansiText.Replace("#033[0m", @"\b0 ");
            ansiText = ansiText.Replace("#033[34m", @"\cf1 ");
            ansiText = ansiText.Replace("#033[0m", @"\cf0 ");
            ansiText = ansiText.Replace("#015", @"\line ");
            return @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}" + ansiText + "}";
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

        private void testConnnectionMenu_Click(object sender, EventArgs e)
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

        private async void btnFetchOutput_Click(object sender, EventArgs e)
        {
            mainPanel.Enabled = false;
            logPanel.Enabled = false;
            connectionMenu.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
            try
            {
                List<string> models = await AWS_Helper.GetTrainingJobOutputList(s3Client, SAGEMAKER_BUCKET);

                if (models != null)
                {
                    outputListComboBox.Items.Clear(); 

                    foreach (var obj in models)
                    {
                        outputListComboBox.Items.Add(obj); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                mainPanel.Enabled = true;
                logPanel.Enabled = true;
                connectionMenu.Enabled = true;
                Cursor = Cursors.Default;
                outputListComboBox.Enabled = true;
            }
        }

        private void modelListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (outputListComboBox.GetItemText(hyperparamsDropdown.SelectedItem) != null)
            {
                string trainingJobOuputs = outputListComboBox.GetItemText(outputListComboBox.SelectedItem);
                outputKey = $"training-jobs/{trainingJobOuputs}/output/output.tar.gz";
                modelKey = $"training-jobs/{trainingJobOuputs}/output/model.tar.gz";
                btnDownloadModel.Enabled = true;
            }
        }

        private void testConnectionMenu_Click(object sender, EventArgs e)
        {
            try
            {
                AWS_Helper.TestSageMakerClient(amazonSageMakerClient);
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

            var createConnectionForm = new CreateConnectionForm(development, this);
            createConnectionForm.FormClosed += OtherForm_FormClosed;
            createConnectionForm.Show();
            this.Enabled = false;
            this.TopLevel = false;
        }

        private void closeConnectionMenu_Click(object sender, EventArgs e)
        {
            UserConnectionInfo.Instance.Reset();
            var t = new Thread(() => Application.Run(new CreateConnectionForm(development, this)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            this.Close();
        }

        private void instancesDropdown_SelectedValueChanged(object sender, EventArgs e)
        {
            if (instancesDropdown.GetItemText(instancesDropdown.SelectedItem) != null)
            {
                selectedInstance = instancesDropdown.GetItemText(instancesDropdown.SelectedItem);
                btnTraining.Enabled = true;
            }
            else
            {
                btnTraining.Enabled = false;
            }
        }
    }
}
