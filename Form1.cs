using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using Amazon.CloudWatchLogs;
using Amazon.IdentityManagement;
using System.Linq;
using LSC_Trainer.Functions;
using System.Threading;
using System.Configuration;
using Amazon.ServiceQuotas;
using Amazon.EC2;


namespace LSC_Trainer
{
    /// <summary>
    /// Represents the main form of the application. Also the Training Job form.
    /// </summary>
    public partial class MainForm : Form, IUIUpdater
    {
        private delegate void SetProgressCallback(int percentDone);
        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonS3Client s3Client;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;
        private AmazonServiceQuotasClient serviceQuotasClient;
        private Utility utility = new Utility();
        private TrainingJobExecutor executor;
        private AmazonIdentityManagementServiceClient identityManagementServiceClient;

        private string USERNAME;
        private string ACCOUNT_ID;
        private string ACCESS_KEY;
        private string SECRET_KEY;
        private string REGION;
        private string ROLE_ARN;

        private string ECR_URI;
        private string IMAGE_TAG;
        private string SAGEMAKER_BUCKET;
        private string DEFAULT_DATASET_URI;
        private string CUSTOM_UPLOADS_URI;
        private string DESTINATION_URI;

        private readonly string SAGEMAKER_INPUT_DATA_PATH = "/opt/ml/input/data/";
        private readonly string SAGEMAKER_OUTPUT_DATA_PATH = "/opt/ml/output/data/";

        private string datasetPath;
        private bool isFile;
        private string folderOrFileName;
        
        private string trainingJobName;

        private string outputKey;
        private string modelKey;

        public bool development;

        private string rootCustomUploadsURI;
        private string selectedInstance;
        private CustomHyperParamsForm customHyperParamsForm;

        private LSC_Trainer.Functions.IFileTransferUtility fileTransferUtility;

        private List<string> supportedInstances = new List<string>()
        {
            "ml.p3.2xlarge","ml.g4dn.xlarge","ml.g4dn.2xlarge","ml.g4dn.4xlarge","ml.g4dn.8xlarge", "ml.g4dn.12xlarge","ml.p3.8xlarge","ml.p3.16xlarge"
        };

        private Dictionary<string, int> instanceToGpuCount = new Dictionary<string, int>
        {
            { "ml.p3.2xlarge", 1 },
            { "ml.g4dn.xlarge", 1 },
            { "ml.g4dn.2xlarge", 1 },
            { "ml.g4dn.4xlarge", 1 },
            { "ml.g4dn.8xlarge", 1 },
            { "ml.g4dn.12xlarge", 4 },
            { "ml.p3.8xlarge", 4 },
            { "ml.p3.16xlarge", 8 }
        };
        private int idealBatchSize = 16;
        private int gpuCount = 0;

        private string dataConfig;

        /// <summary>
        /// Initializes the MainForm, UserConnectionInfo, and Form Controls.
        /// </summary>
        /// <param name="development">A boolean indicating whether the application is running in development mode.</param>
        public MainForm(bool development)
        {
            InitializeComponent();
            this.development = development;

            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            if (development)
            {
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string ENV_PATH = Path.Combine(projectDirectory, ".env").Replace("\\", "/");
                DotNetEnv.Env.Load(ENV_PATH);

                UserConnectionInfo.AccountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
                UserConnectionInfo.AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
                UserConnectionInfo.SecretKey = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
                UserConnectionInfo.Region = Environment.GetEnvironmentVariable("REGION");
                UserConnectionInfo.RoleArn = Environment.GetEnvironmentVariable("ROLE_ARN");
                UserConnectionInfo.EcrUri = Environment.GetEnvironmentVariable("INTELLISYS_ECR_URI");
                UserConnectionInfo.SagemakerBucket = Environment.GetEnvironmentVariable("SAGEMAKER_BUCKET");
                UserConnectionInfo.DefaultDatasetURI = Environment.GetEnvironmentVariable("DEFAULT_DATASET_URI");
                UserConnectionInfo.CustomUploadsURI = Environment.GetEnvironmentVariable("CUSTOM_UPLOADS_URI");
                UserConnectionInfo.DestinationURI = Environment.GetEnvironmentVariable("DESTINATION_URI");
                MessageBox.Show("Established Connection using ENV for Development", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!development && UserConnectionInfo.AccountId == null && UserConnectionInfo.AccessKey == null && UserConnectionInfo.SecretKey == null && UserConnectionInfo.Region == null && UserConnectionInfo.RoleArn == null)
            {
                MessageBox.Show("No connection established. Please create a connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var t = new Thread(() => Application.Run(new CreateConnectionForm(this)));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                this.Close();
                Console.WriteLine($"Establishing Connection...");
            }
            
            logBox.Rtf = @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}";
            btnTraining.Enabled = false;
            btnUploadToS3.Enabled = false;
            btnDownloadModel.Enabled = false;

            executor = new TrainingJobExecutor(this);
            fileTransferUtility = new FileTransferUtility(this);
            MessageBox.Show("Established Connection with UserConnectionInfo", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            InitializeClient();
            InitializeInputs();
        }

        /// <summary>
        /// Initializes the User Credentials, necessary URIs, region, Amazon SageMaker, Amazon S3, and Amazon CloudWatch Logs clients with UserConnectionInfo.
        /// </summary>
        public void InitializeClient()
        {
            ACCOUNT_ID = UserConnectionInfo.AccountId;
            ACCESS_KEY = UserConnectionInfo.AccessKey;
            SECRET_KEY = UserConnectionInfo.SecretKey;
            REGION = UserConnectionInfo.Region;
            ROLE_ARN = UserConnectionInfo.RoleArn;
            var ecrUriAndImageTag = GetECRUri();
            ECR_URI = ecrUriAndImageTag.Item1 ?? UserConnectionInfo.EcrUri;
            IMAGE_TAG = ecrUriAndImageTag.Item2 ?? "latest";
            ECR_URI = $"{ECR_URI}:{IMAGE_TAG}";
            SAGEMAKER_BUCKET = UserConnectionInfo.SagemakerBucket;
            DEFAULT_DATASET_URI = UserConnectionInfo.DefaultDatasetURI;
            CUSTOM_UPLOADS_URI = UserConnectionInfo.CustomUploadsURI;
            DESTINATION_URI = UserConnectionInfo.DestinationURI;
            RegionEndpoint region = RegionEndpoint.GetBySystemName(REGION);
            amazonSageMakerClient = new AmazonSageMakerClient(ACCESS_KEY, SECRET_KEY, region);
            s3Client = new AmazonS3Client(ACCESS_KEY, SECRET_KEY, region);
            cloudWatchLogsClient = new AmazonCloudWatchLogsClient(ACCESS_KEY, SECRET_KEY, region);
            serviceQuotasClient = new AmazonServiceQuotasClient(ACCESS_KEY, SECRET_KEY, region);
            identityManagementServiceClient = new AmazonIdentityManagementServiceClient(ACCESS_KEY, SECRET_KEY, region);
            AWS_Helper.CheckCredentials(identityManagementServiceClient);
            USERNAME = UserConnectionInfo.UserName;
            rootCustomUploadsURI = CUSTOM_UPLOADS_URI;

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

        /// <summary>
        /// Initializes the input fields with default values based on the default dataset URI.
        /// </summary>
        public void InitializeInputs()
        {
            imgSizeDropdown.Text = "1280";
            txtBatchSize.Text = "16";
            txtEpochs.Text = "1";
            txtWeights.Text = "yolov5n6.pt";
            dataConfig = "sample.yaml";
            hyperparamsDropdown.Text = "hyp.no-augmentation.yaml";
            txtPatience.Text = "100";
            txtWorkers.Text = "8";
            txtOptimizer.Text = "SGD";
            txtGpuCount.Text = "1";
            txtInstanceCount.Text = "1";

            instancesDropdown_SetValues();
        }

        /// <summary>
        /// Retrieves the URI of the first repository in Amazon Elastic Container Registry (ECR).
        /// </summary>
        /// <returns>The URI of the first repository in ECR, or null if no repositories are found.</returns>
        public (string, string) GetECRUri()
        {
            return AWS_Helper.GetFirstRepositoryUriAndImageTag(ACCESS_KEY, SECRET_KEY, RegionEndpoint.GetBySystemName(REGION));
        }

        /// <summary>
        /// Event handler for the Click event of the "Select Dataset (Zip)" button.
        /// Opens a file dialog to allow the user to select a ZIP file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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
                    lblZipFile.Text = Path.GetFileNameWithoutExtension(datasetPath);
                    datasetListComboBox.Text = "";

                    MessageBox.Show($"Selected file: {datasetPath}");
                    isFile = true;
                    btnUploadToS3.Enabled = true;
                    btnTraining.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Event handler for the Click event of the "Select Dataset (Folder)" button.
        /// Opens a folder browser dialog to allow the user to select a folder.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder";
                folderBrowserDialog.ShowNewFolderButton = false;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    datasetPath = folderBrowserDialog.SelectedPath;

                    lblZipFile.Text = datasetPath.Split('\\').Last();
                    datasetListComboBox.Text = "";

                    MessageBox.Show($"Selected folder: {datasetPath}");
                    isFile = false;
                    btnUploadToS3.Enabled = true;
                    btnTraining.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Event handler for the Click event of the "Upload Dataset" button.
        /// Uploads the selected dataset to an Amazon S3 bucket if available.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnUploadToS3_Click(object sender, EventArgs e)
        {
            if(datasetPath != null)
            {
                folderOrFileName = datasetPath.Split('\\').Last();
                DialogResult result = MessageBox.Show($"Do you want to upload {folderOrFileName} to s3 bucket?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    backgroundWorker.RunWorkerAsync();
                    mainPanel.Enabled = false;
                    logPanel.Enabled = false;
                    connectionMenu.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    lscTrainerMenuStrip.Cursor = Cursors.Default;
                    trainingStatusBox.Text = "Uploading to S3";
                    descBox.Text = "Your dataset is being uploaded to S3. Please wait...";
                }
            }
            else
            {
                MessageBox.Show("No file to upload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for the Click event of the "Train" button.
        /// Clears log box and sets training parameters before initiating a training job.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnTraining_Click(object sender, EventArgs e)
        {
             try
            {
                if (ValidateTrainingParameters(imgSizeDropdown.Text, txtBatchSize.Text, txtEpochs.Text, txtWeights.Text, hyperparamsDropdown.Text
                , txtPatience.Text, txtWorkers.Text, txtOptimizer.Text, txtGpuCount.Text, txtInstanceCount.Text))
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
                            out string device,
                            out string instanceCount);

                    string modifiedInstance = selectedInstance.ToUpper().Replace(".", "").Replace("ML", "").Replace("XLARGE", "XL");
                    string imageTagFirstThree = IMAGE_TAG.Length >= 3 ? IMAGE_TAG.Substring(0, 3) : IMAGE_TAG;
                    string cleanData = Regex.Replace(data, @"[^0-9a-zA-Z\-]", string.Empty);
                    string cleanUserName = Regex.Replace(USERNAME, @"[^0-9a-zA-Z\-]", string.Empty);
                    string dateTime = DateTime.Now.ToString("yyMMddHHmm");
                    trainingJobName = string.Format("{0}-{1}-{2}-{3}", cleanUserName, cleanData, modifiedInstance, dateTime);

                    CreateTrainingJobRequest trainingRequest = executor.CreateTrainingRequest(
                    img_size, batch_size, epochs, weights, data, hyperparameters, patience, workers, optimizer, device, instanceCount, selectedInstance, CUSTOM_UPLOADS_URI, DEFAULT_DATASET_URI, ECR_URI, SAGEMAKER_INPUT_DATA_PATH, SAGEMAKER_OUTPUT_DATA_PATH, ROLE_ARN, DESTINATION_URI, trainingJobName, customHyperParamsForm);

                    
                    this.Text = trainingJobName;
                    bool hasCustomUploads = utility.HasCustomUploads(CUSTOM_UPLOADS_URI);
                    if (hasCustomUploads)
                    {
                        InitiateTrainingJob(trainingRequest);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }

        /// <summary>
        /// Event handler for the Click event of the "Download Model" button.
        /// Downloads the trained model and results from Amazon S3 to a selected local folder.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="Exception"> Thrown when an error occurs while downloading the model.</exception>
        private async void btnDownloadModel_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder to save the results and model";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedLocalPath = folderBrowserDialog.SelectedPath;

                    DialogResult result = MessageBox.Show($"Do you want to save the results and model to {selectedLocalPath + $"\\{trainingJobName ?? outputListComboBox.Text}"} ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            mainPanel.Enabled = false;
                            logPanel.Enabled = false;
                            connectionMenu.Enabled = false;
                            Cursor = Cursors.WaitCursor;
                            lscTrainerMenuStrip.Cursor = Cursors.Default;
                            string outputResponse = await fileTransferUtility.DownloadObjects(s3Client, SAGEMAKER_BUCKET, outputKey, selectedLocalPath + $"/{trainingJobName ?? outputListComboBox.Text}");
                            DisplayLogMessage(outputResponse);
                            string modelResponse = await fileTransferUtility.DownloadObjects(s3Client, SAGEMAKER_BUCKET, modelKey, selectedLocalPath + $"/{trainingJobName ?? outputListComboBox.Text}");
                            DisplayLogMessage(modelResponse);
                        }catch(AmazonS3Exception s3Exception)
                        {
                            MessageBox.Show($"Error in downloading model: {s3Exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// Event handler for the DoWork event of the background worker.
        /// Performs background work, such as uploading files or folders to Amazon S3 and progress tracking.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An instance of DoWorkEventArgs containing event data.</param>
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            CUSTOM_UPLOADS_URI = rootCustomUploadsURI;

            if (isFile)
            {
                fileTransferUtility.UnzipAndUploadToS3(s3Client, SAGEMAKER_BUCKET, $"users/{USERNAME}/custom-uploads/", datasetPath, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
                CUSTOM_UPLOADS_URI = CUSTOM_UPLOADS_URI + Path.GetFileNameWithoutExtension(datasetPath) + "/";
            }
            else
            {
                fileTransferUtility.UploadFolderToS3(s3Client, datasetPath, $"users/{USERNAME}/custom-uploads/" + folderOrFileName, SAGEMAKER_BUCKET, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
                CUSTOM_UPLOADS_URI = CUSTOM_UPLOADS_URI + folderOrFileName + "/";
            }
        }

        /// <summary>
        /// Event handler for the ProgressChanged event of the background worker.
        /// Updates the progress bar value based on the reported progress percentage.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An instance of ProgressChangedEventArgs containing event data.</param>
        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= progressBar.Minimum && e.ProgressPercentage <= progressBar.Maximum)
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        /// <summary>
        /// Event handler for the RunWorkerCompleted event of the background worker.
        /// Displays a message indicating that the upload has completed, and resets UI elements.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An instance of RunWorkerCompletedEventArgs containing event data.</param>
        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Upload completed!");
            progressBar.Value = 0;
            mainPanel.Enabled = true;
            logPanel.Enabled = true;
            connectionMenu.Enabled = true;
            Cursor = Cursors.Default;
            trainingStatusBox.Text = "";
            descBox.Text = "";
            btnTraining.Enabled = true ? utility.HasCustomUploads(CUSTOM_UPLOADS_URI) && instancesDropdown.SelectedItem != null : false;
            btnUploadToS3.Enabled = false;
        }

        /// <summary>
        /// Event handler to select all text when a control receives the click event.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectAllTextOnClick(object sender, EventArgs e)
        {
            sender.GetType().GetMethod("SelectAll")?.Invoke(sender, null);
        }

        /// <summary>
        /// Sets the training parameters based on the text properties of UI controls.
        /// </summary>
        /// <param name="img_size">Output parameter to store the image size.</param>
        /// <param name="batch_size">Output parameter to store the batch size.</param>
        /// <param name="epochs">Output parameter to store the number of epochs.</param>
        /// <param name="weights">Output parameter to store the weights.</param>
        /// <param name="data">Output parameter to store the data.</param>
        /// <param name="hyperparameters">Output parameter to store the hyperparameters.</param>
        /// <param name="patience">Output parameter to store the patience.</param>
        /// <param name="workers">Output parameter to store the number of workers.</param>
        /// <param name="optimizer">Output parameter to store the optimizer.</param>
        /// <param name="device">Output parameter to store the device.</param>
        /// <param name="instanceCount">Output parameter to store the instance count.</param>
        private void SetTrainingParameters(out string img_size, out string batch_size, out string epochs, out string weights, out string data, out string hyperparameters, out string patience, out string workers, out string optimizer, out string device, out string instanceCount)
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
            instanceCount = "";

            if (imgSizeDropdown.Text != "") img_size = imgSizeDropdown.Text;

            if (txtBatchSize.Text != "") batch_size = txtBatchSize.Text;

            if (txtEpochs.Text != "") epochs = txtEpochs.Text;

            if (txtWeights.Text != "") weights = txtWeights.Text;

            if (lblZipFile.Text != "") dataConfig = lblZipFile.Text;

            data = dataConfig;

            if (hyperparamsDropdown.Text != "") hyperparameters = hyperparamsDropdown.Text;

            if (txtPatience.Text != "") patience = txtPatience.Text;

            if (txtWorkers.Text != "") workers = txtWorkers.Text;

            if (txtOptimizer.Text != "") optimizer = txtOptimizer.Text;

            if (txtGpuCount.Text != "")
            {
                if(txtGpuCount.Text == "cpu")
                {
                    device = "cpu";
                }
                else
                {
                    int gpuCount = int.Parse(txtGpuCount.Text);
                    device = gpuCount > 0 ? string.Join(",", Enumerable.Range(0, gpuCount)) : "cpu";
                }
                
            }

            if (txtInstanceCount.Text != "") instanceCount = txtInstanceCount.Text;
        }

        /// <summary>
        /// Enables or disables various input controls based on the provided intent.
        /// </summary>
        /// <param name="intent">True to enable the input controls, false to disable them.</param>
        private void InputsEnabler(bool intent)
        {
            imgSizeDropdown.Enabled = intent;
            txtBatchSize.Enabled = intent;
            txtEpochs.Enabled = intent;
            txtWeights.Enabled = intent;
            hyperparamsDropdown.Enabled = intent;
            txtPatience.Enabled = intent;
            txtWorkers.Enabled = intent;
            txtOptimizer.Enabled = intent;
            txtGpuCount.Enabled = intent;
            txtInstanceCount.Enabled = intent;
            btnSelectDataset.Enabled = intent;
            btnSelectFolder.Enabled = intent;
            btnUploadToS3.Enabled = intent;
            btnTraining.Enabled = intent;
            datasetListComboBox.Enabled = intent;
            outputListComboBox.Enabled = intent;
            instancesDropdown.Enabled = intent;
            btnFetchDatasets.Enabled = intent;
            btnFetchOutput.Enabled = intent;
            btnDownloadModel.Enabled = intent;
            lblZipFile.Enabled = intent;
            logBox.UseWaitCursor = !intent;
        }

        /// <summary>
        /// Initiates a training job using the provided training request and tracks it using the CloudWatch Logs client.
        /// </summary>
        /// <param name="trainingRequest">The request object for starting the training job.</param>
        /// <exception cref="Exception">Thrown when an error occurs while initiating the training job.</exception>
        private async void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest)
        {
            try
            {
                SetUIState(true);
                outputListComboBox.Text = "";
                string datasetKey = CUSTOM_UPLOADS_URI.Replace($"s3://{SAGEMAKER_BUCKET}/", "");
                await executor.InitiateTrainingJob(trainingRequest, cloudWatchLogsClient, amazonSageMakerClient, s3Client, fileTransferUtility, datasetKey, SAGEMAKER_BUCKET, utility.HasCustomUploads(CUSTOM_UPLOADS_URI));
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating training job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnTraining.Enabled = true;
                return;
            }finally
            {
                SetUIState(false);
                btnUploadToS3.Enabled = false;
                outputListComboBox.Text = this.Text;
                outputKey = $"users/{USERNAME}/training-jobs/{trainingJobName}/output/output.tar.gz";
                modelKey = $"users/{USERNAME}/training-jobs/{trainingJobName}/output/model.tar.gz";
                datasetPath = null;
            }
        }

        /// <summary>
        /// Event handler for the click event of the menu item "New Training Job" to create a new training job form.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void NewTrainingJobMenu_Click(object sender, EventArgs e)
        {
            var t = new Thread(() => Application.Run(new MainForm(development)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Event handler for the selection change committed event of the image size dropdown.
        /// Sets the weights text box to the weight file based on the selected image size, or default value if weight file 
        /// is null.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ImgSizeDropdown_SelectionChangeCommitted(object sender, EventArgs e)
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

        /// <summary>
        /// Event handler for the SelectedValueChanged event of the hyperparameters dropdown.
        /// Opens the custom hyperparameters form if the selected item is "Custom". 
        /// Otherwise, sets the text of the dropdown to the selected item.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HyperparamsDropdown_SelectedValueChanged(object sender, EventArgs e)
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

        /// <summary>
        /// Event handler for the click event of the Help menu item.
        /// Opens the help form to display information about the application.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HelpMenu_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var helpForm = new HelpForm();

            helpForm.FormClosed += OtherForm_FormClosed;
            helpForm.Show();
        }

        /// <summary>
        /// Event handler for the FormClosed event of another form.
        /// Re-enables the main form when the other form is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OtherForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Event handler for the click event of the Test Connection menu item.
        /// Lists the models in the Amazon SageMaker service to test the connection.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="Exception">Thrown when an error occurs while testing the connection.</exception>
        private void TestConnnectionMenu_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Event handler for the click event of the Fetch Output button.
        /// Fetches the output list from the Amazon S3 bucket and populates the output list combo box with the list of models.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="Exception">Thrown when an error occurs while fetching the output list.</exception>
        private async void btnFetchOutput_Click(object sender, EventArgs e)
        {
            mainPanel.Enabled = false;
            logPanel.Enabled = false;
            connectionMenu.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
            try
            {
                List<string> jobName = await AWS_Helper.GetTrainingJobOutputList(s3Client, SAGEMAKER_BUCKET);

                if (jobName != null && jobName.Count > 0)
                {
                    outputListComboBox.Items.Clear();
                    outputKey = $"users/{USERNAME}/training-jobs/{jobName[0]}/output/output.tar.gz";
                    outputListComboBox.SelectedText = jobName[1];
                    foreach (var obj in jobName)
                    {
                        outputListComboBox.Items.Add(obj); 
                    }
                }
                else
                {
                    outputListComboBox.Items.Add("No items"); 
                    btnDownloadModel.Enabled = false; 
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

        /// <summary>
        /// Event handler for the SelectedValueChanged event of the model list combo box.
        /// Sets the output key and model key based on the selected model, and enables the download model button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void outputListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (outputListComboBox.GetItemText(hyperparamsDropdown.SelectedItem) != null && outputListComboBox.Text != "")
            {
                string trainingJobOuputs = outputListComboBox.GetItemText(outputListComboBox.SelectedItem);
                outputKey = $"users/{USERNAME}/training-jobs/{trainingJobOuputs}/output/output.tar.gz";
                modelKey = $"users/{USERNAME}/training-jobs/{trainingJobOuputs}/output/model.tar.gz";
                btnDownloadModel.Enabled = true;
            }
            else
            {
                btnDownloadModel.Enabled = false;
            }
        }

        /// <summary>
        /// Event handler for the click event of the Test Connection menu item.
        /// Tests the connection to the Amazon SageMaker service by listing the models in the service.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="Exception">Thrown when an error occurs while testing the connection.</exception>
        private void testConnectionMenu_Click(object sender, EventArgs e)
        {
            try
            {
                AWS_Helper.CheckCredentials(identityManagementServiceClient);
                MessageBox.Show("Connection successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Unexpected error: {error.Message}");
                MessageBox.Show($"Connection failed: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for the click event of the Create Connection menu item.
        /// Creates a new connection form to establish a new connection to the Amazon SageMaker service.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void createConnectionMenu_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var createConnectionForm = new CreateConnectionForm(this);
            createConnectionForm.FormClosed += OtherForm_FormClosed;
            createConnectionForm.Show();
        }

        /// <summary>
        /// Event handler for the click event of the Close Connection menu item.
        /// Logs out the user and resets the connection information, then opens the create connection form to establish a new connection.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void closeConnectionMenu_Click(object sender, EventArgs e)
        {
            UserConnectionInfo.Instance.Reset();
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(config.FilePath))
            {
                File.Delete(config.FilePath);
            }

            var t = new Thread(() => Application.Run(new CreateConnectionForm()));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            this.Close();
        }

        /// <summary>
        /// Sets the values of the instances dropdown by clearing existing items and adding new ones retrieved from AWS.
        /// </summary>
        private async void instancesDropdown_SetValues() {
            instancesDropdown.Items.Clear();

            List<(string instance, double value)> instances = await AWS_Helper.GetAllSpotTrainingQuotas(serviceQuotasClient);

            foreach(var instance in instances)
            {
                instancesDropdown.Items.Add(instance);
            }

        }

        /// <summary>
        /// Event handler for the SelectedValueChanged event of the instancesDropdown control.
        /// Updates the selectedInstance variable and enables/disables the btnTraining button accordingly.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs object that contains additional information about the event, 
        /// if any is relevant.</param>
        private void instancesDropdown_SelectedValueChanged(object sender, EventArgs e)
        {
            if (instancesDropdown.SelectedItem != null)
            {
                var selectedItem = ((string, double))instancesDropdown.SelectedItem;
                selectedInstance = selectedItem.Item1;

                if (instanceToGpuCount.TryGetValue(selectedInstance, out int gpuCount))
                {
                    txtGpuCount.Text = gpuCount.ToString();
                }
                else
                {
                    txtGpuCount.Text = "0";
                }
                CalculateBatchSize();
                btnTraining.Enabled = true ? utility.HasCustomUploads(CUSTOM_UPLOADS_URI) : false;
            }
            else
            {
                btnTraining.Enabled = false;
            }
        }

        /// <summary>
        /// This method is triggered whenever the value of the txtInstanceCount textbox changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs object that contains additional information about the event, 
        /// if any is relevant.</param>
        private void txtInstanceCount_ValueChanged(object sender, EventArgs e)
        {
            if (txtInstanceCount.Text != null)
            {
                int instanceCount;
                
                if (txtInstanceCount.Text == "")
                {
                    MessageBox.Show("Instance count cannot be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInstanceCount.Text = "1";
                }
                else if (!Int32.TryParse(txtInstanceCount.Text, out instanceCount))
                {
                    MessageBox.Show("Instance count must be an integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtInstanceCount.Text = "1";
                }
                else if (instanceCount <= 0)
                {
                    MessageBox.Show("Instance count must be greater than 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtInstanceCount.Text = "1";
                }
                else
                {
                    CalculateBatchSize();
                }
            }
        }

        private void txtGpuCount_ValueChanged(object sender, EventArgs e)
        {
            CalculateBatchSize();
        }

        /// <summary>
        /// Validates the user input in the txtGpuCount textbox, which specifies the number of GPUs to use for training.
        /// </summary>
        /// <returns>True if the input is valid (either a positive integer or "cpu"), False otherwise.</returns>
        private bool txtGpuCount_Validate()
        {
            if (txtGpuCount.Text != null)
            {
                // If the user wants to use only the CPU
                if (txtGpuCount.Text.ToLower() == "cpu")
                {
                    CalculateBatchSize();
                    txtGpuCount.Text = "cpu";
                    return true;
                }
                else if (Int32.TryParse(txtGpuCount.Text, out int x))
                {
                    if (x < 0)
                    {
                        MessageBox.Show("GPU count must be 0 or greater than 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (x == 0)
                    {
                        CalculateBatchSize();
                        txtGpuCount.Text = "cpu";
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("GPU count must be an integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the optimal batch size for a specific task.
        /// </summary>
        private void CalculateBatchSize()
        {
            if (instancesDropdown.SelectedItem != null)
            {
                var selectedItem = ((string, double))instancesDropdown.SelectedItem;
                string instance = selectedItem.Item1;
                int instanceCount = Int32.Parse(txtInstanceCount.Text);
                int.TryParse(txtGpuCount.Text, out gpuCount);
                idealBatchSize = 16;

                if (instanceCount == 1 && (gpuCount == 0 || (txtGpuCount.Text.ToLower() == "cpu") || int.Parse(txtGpuCount.Text) == 0) && !supportedInstances.Contains(instance))
                {
                    idealBatchSize = -1;
                }
                else
                {
                    idealBatchSize = 16 * instanceCount * gpuCount;
                }

                txtBatchSize.Text = idealBatchSize.ToString();
            }
        }

        /// <summary>
        /// Validates the provided training parameters to ensure they are in a valid format and meet specific requirements.
        /// </summary>
        /// <param name="img_size">The image size for training (e.g., "224x224").</param>
        /// <param name="batch_size">The number of samples to process in each batch during training.</param>
        /// <param name="epochs">The number of times to iterate through the entire training dataset.</param>
        /// <param name="weights">The path to pre-trained weights to initialize the model (or "None" if not used).</param>
        /// <param name="hyperparameters">A string representation of additional hyperparameters for the training process.</param>
        /// <param name="patience">The number of epochs with no improvement to wait before early stopping (or "0" to disable).</param>
        /// <param name="workers">The number of worker processes to use for data loading (e.g., for parallel processing).</param>
        /// <param name="optimizer">The name of the optimizer algorithm to use for training (e.g., "adam").</param>
        /// <param name="device">The device to use for training (e.g., "cuda" for GPU, "cpu" for CPU).</param>
        /// <param name="instanceCount">The number of training instances (optional).</param>
        /// <returns>True if all parameters are valid, False otherwise.</returns>
        private bool ValidateTrainingParameters(string img_size, string batch_size, string epochs, string weights, string hyperparameters, string patience, string workers, string optimizer, string device, string instanceCount)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"Image size", img_size},
                {"Batch size", batch_size},
                {"Epochs", epochs},
                {"Weights", weights},
                {"Hyperparameters", hyperparameters},
                {"Patience", patience},
                {"Workers", workers},
                {"Optimizer", optimizer},
                {"Device", device},
                {"Instance count", instanceCount}
            };
            
            Dictionary<string, string> intFields = new Dictionary<string, string>()
            {
                {"Epochs", epochs},
                {"Patience", patience},
                {"Workers", workers},
                {"Instance count", instanceCount}
            };

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Value))
                {
                    MessageBox.Show($"{parameter.Key} cannot be null or empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            foreach (KeyValuePair<string, string> intField in intFields)
            {
                if (!Int32.TryParse(intField.Value, out int number) || number <= 0)
                {
                    MessageBox.Show($"{intField.Key} must be a positive integer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (!Int32.TryParse(txtBatchSize.Text, out int batchSize))
            {
                MessageBox.Show("Batch size must be an integer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (int.Parse(txtBatchSize.Text) < idealBatchSize)
            {
                MessageBox.Show($"Batch size cannot be lesser than the ideal batch size {idealBatchSize}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (batchSize == 0)
            {
                MessageBox.Show("Batch size cannot be 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!txtGpuCount_Validate())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is triggered when the main form is being closed.
        /// It performs cleanup tasks by disposing of any managed resources held by the executor and fileTransferUtility objects.
        /// </summary>
        /// <param name="sender">The object that raised the event (the main form in this case).</param>
        /// <param name="e">An EventArgs object that provides additional information about the form closing event (usually not used here).</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            executor?.Dispose();
            fileTransferUtility?.Dispose();
        }

        /// <summary>
        /// Sets the enabled state of various UI elements based on whether a training process is ongoing.
        /// </summary>
        /// <param name="isTraining">True if training is in progress, False otherwise.</param>
        public void SetUIState(bool isTraining)
        {
            InputsEnabler(!isTraining);
            connectionMenu.Enabled = !isTraining;
            logPanel.Enabled = !isTraining;
            Cursor = isTraining ? Cursors.WaitCursor : Cursors.Default;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Updates the training status label with the provided instance type, training duration, status, and description.
        /// </summary>
        /// <param name="instanceType">The instance type used for training.</param>
        /// <param name="trainingDuration">The duration of the training.</param>
        /// <param name="status">The status of the training job.</param>
        /// <param name="description">A description of the training job status.</param>
        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateTrainingStatus(instanceType, trainingDuration, status, description)));
                    return;
                }

                instanceTypeBox.Text = instanceType;
                trainingDurationBox.Text = trainingDuration;
                trainingStatusBox.Text = status;
                descBox.Text = description;
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Main form is closed. Cannot update training status.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the training duration label with the provided training duration value.
        /// </summary>
        /// <param name="trainingDuration">The duration of the training.</param>
        public void UpdateTrainingStatus(string trainingDuration)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateTrainingStatus(trainingDuration)));
                    return;
                }
                trainingDurationBox.Text = trainingDuration;
            }
            catch(ObjectDisposedException)
            {
                Console.WriteLine("Main form is closed. Cannot update training status.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the training status and description labels with the provided values.
        /// </summary>
        /// <param name="status">The status of the training.</param>
        /// <param name="description">The description of the training.</param>
        public void UpdateTrainingStatus(string status, string description)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateTrainingStatus(status, description)));
                    return;
                }
                trainingStatusBox.Text = status;
                descBox.Text = description;
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Main form is closed. Cannot update training status.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the download status of the output with the provided values.
        /// </summary>
        /// <param name="percentage">The percentage of the download process.</param>
        public void UpdateDownloadStatus(int percentage)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    if (percentage >= downloadProgressBar.Minimum && percentage <= downloadProgressBar.Maximum)
                    {
                        this.Invoke(new Action(() => UpdateDownloadStatus(percentage)));
                        return;
                    }
                }
                
                downloadProgressBar.Value = percentage;

                if(percentage >= 100)
                    downloadProgressBar.Value = 0;
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Main form is closed. Cannot update download progress bar.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays the provided log message in the associated RichTextBox control.
        /// </summary>
        /// <param name="logMessage">The log message to be displayed.</param>
        public void DisplayLogMessage(string logMessage)
        {
            try
            {
                string rtfMessage = ConvertAnsiToRtf(logMessage);

                // Remove the start and end of the RTF document from the message
                rtfMessage = rtfMessage.Substring(rtfMessage.IndexOf('}') + 1);
                rtfMessage = rtfMessage.Substring(0, rtfMessage.LastIndexOf('}'));

                // Append the RTF message at the end of the existing RTF text
                Action log = () =>
                {
                    logBox.Rtf = logBox.Rtf.Insert(logBox.Rtf.LastIndexOf('}'), rtfMessage);

                    // Scroll to the end to show the latest log messages
                    logBox.SelectionStart = logBox.Text.Length;
                    logBox.ScrollToCaret();
                };
                if (logBox.InvokeRequired)
                {
                    // Invoke on the UI thread
                    logBox.Invoke(log);
                }
                else
                {
                    // No invoke required, execute directly
                    log();
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Main form is closed. Cannot update training status.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Converts ANSI formatted text to RTF format for a more readable display in a RichTextBox control.
        /// </summary>
        /// <param name="ansiText">The ANSI formatted text to be converted.</param>
        /// <returns>The RTF formatted string.</returns>
        public string ConvertAnsiToRtf(string ansiText)
        {
            ansiText = ansiText.Replace("#033[1m", @"\b ");
            ansiText = ansiText.Replace("#033[0m", @"\b0 ");
            ansiText = ansiText.Replace("#033[34m", @"\cf1 ");
            ansiText = ansiText.Replace("#033[0m", @"\cf0 ");
            ansiText = ansiText.Replace("#015", @"\line ");
            return @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}" + ansiText + "}";
        }

        public void SetLogPanelVisibility(bool visibility)
        {
            logPanel.Visible = visibility;
        }

        /// <summary>
        /// This method is triggered when the "Fetch Datasets" button is clicked.
        /// It asynchronously retrieves a list of available datasets from Amazon S3 and populates a combo box with the retrieved dataset names.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs object that provides additional information about the click event.</param>
        private async void btnFetchDatasets_Click(object sender, EventArgs e)
        {
            mainPanel.Enabled = false;
            logPanel.Enabled = false;
            connectionMenu.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
            try
            {
                List<string> datasets = await AWS_Helper.GetAvailableDatasetsList(s3Client, SAGEMAKER_BUCKET);

                datasetListComboBox.Items.Clear();

                if (datasets != null && datasets.Count > 0)
                {

                    foreach (var obj in datasets)
                    {
                        datasetListComboBox.Items.Add(obj);
                    }
                }
                else
                {
                    datasetListComboBox.Items.Add("No items");
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
                datasetListComboBox.Enabled = true;
            }
        }

        /// <summary>
        /// This method is triggered whenever the selected value in the datasetListComboBox changes.
        /// It updates the CUSTOM_UPLOADS_URI variable to include the selected dataset name and sets the lblZipFile text accordingly.
        /// Disables the btnUploadToS3 button initially.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs object that provides additional information about the selection changed event.</param>
        private void datasetListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            CUSTOM_UPLOADS_URI = rootCustomUploadsURI + datasetListComboBox.GetItemText(datasetListComboBox.SelectedItem) + "/";
            lblZipFile.Text = datasetListComboBox.GetItemText(datasetListComboBox.SelectedItem);
            btnUploadToS3.Enabled = false;
            btnTraining.Enabled = true ? utility.HasCustomUploads(CUSTOM_UPLOADS_URI) && instancesDropdown.SelectedItem != null : false;
        }
    }
}
