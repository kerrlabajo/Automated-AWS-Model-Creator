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

        public (string, string) GetECRUri()
        {
            return AWS_Helper.GetFirstRepositoryUriAndImageTag(ACCESS_KEY, SECRET_KEY, RegionEndpoint.GetBySystemName(REGION));
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
                    lblZipFile.Text = datasetPath.Split('\\').Last();
                    datasetListComboBox.Text = "";

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

                    lblZipFile.Text = datasetPath.Split('\\').Last();
                    datasetListComboBox.Text = "";

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
                    trainingJobName = string.Format("{0}-{1}-{2}-{3}-{4}", cleanUserName, cleanData, modifiedInstance, imageTagFirstThree.Replace(".", "-"), dateTime);

                    CreateTrainingJobRequest trainingRequest = executor.CreateTrainingRequest(
                    img_size, batch_size, epochs, weights, data, hyperparameters, patience, workers, optimizer, device, instanceCount, selectedInstance, CUSTOM_UPLOADS_URI, DEFAULT_DATASET_URI, ECR_URI, SAGEMAKER_INPUT_DATA_PATH, SAGEMAKER_OUTPUT_DATA_PATH, ROLE_ARN, DESTINATION_URI, trainingJobName, customHyperParamsForm);

                    
                    this.Text = trainingJobName;
                    bool hasCustomUploads = utility.HasCustomUploads(CUSTOM_UPLOADS_URI);
                    string datasetKey = CUSTOM_UPLOADS_URI.Replace($"s3://{SAGEMAKER_BUCKET}/", "");
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

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            CUSTOM_UPLOADS_URI = rootCustomUploadsURI;

            if (isFile)
            {
                fileTransferUtility.UnzipAndUploadToS3(s3Client, SAGEMAKER_BUCKET, datasetPath, new Progress<int>(percent =>
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
            trainingStatusBox.Text = "";
            descBox.Text = "";
        }

        private void SelectAllTextOnClick(object sender, EventArgs e)
        {
            sender.GetType().GetMethod("SelectAll")?.Invoke(sender, null);
        }

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

        private async void InitiateTrainingJob(CreateTrainingJobRequest trainingRequest)
        {
            try
            {
                SetUIState(true);
                outputListComboBox.Text = "";
                await executor.InitiateTrainingJob(trainingRequest, cloudWatchLogsClient, amazonSageMakerClient, s3Client, fileTransferUtility, datasetPath, SAGEMAKER_BUCKET, utility.HasCustomUploads(CUSTOM_UPLOADS_URI));
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

        private void NewTrainingJobMenu_Click(object sender, EventArgs e)
        {
            var t = new Thread(() => Application.Run(new MainForm(development)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

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

        private void HelpMenu_Click(object sender, EventArgs e)
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

                    foreach (var obj in jobName)
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

        private void outputListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (outputListComboBox.GetItemText(hyperparamsDropdown.SelectedItem) != null && outputListComboBox.Text != "")
            {
                string trainingJobOuputs = outputListComboBox.GetItemText(outputListComboBox.SelectedItem);
                outputKey = $"users/{USERNAME}/training-jobs/{trainingJobOuputs}/output/output.tar.gz";
                modelKey = $"users/ {USERNAME} /training-jobs/{trainingJobOuputs}/output/model.tar.gz";
                btnDownloadModel.Enabled = true;
            }
            else
            {
                btnDownloadModel.Enabled = false;
            }
        }

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

        private void createConnectionMenu_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var createConnectionForm = new CreateConnectionForm(this);
            createConnectionForm.FormClosed += OtherForm_FormClosed;
            createConnectionForm.Show();
        }

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

        private async void instancesDropdown_SetValues() {
            instancesDropdown.Items.Clear();

            List<(string instance, double value)> instances = await AWS_Helper.GetAllSpotTrainingQuotas(serviceQuotasClient);

            foreach(var instance in instances)
            {
                instancesDropdown.Items.Add(instance);
            }

        }
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
                btnTraining.Enabled = true;
            }
            else
            {
                btnTraining.Enabled = false;
            }
        }

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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            executor?.Dispose();
            fileTransferUtility?.Dispose();
        }

        public void SetUIState(bool isTraining)
        {
            InputsEnabler(!isTraining);
            connectionMenu.Enabled = !isTraining;
            logPanel.Enabled = !isTraining;
            Cursor = isTraining ? Cursors.WaitCursor : Cursors.Default;
            lscTrainerMenuStrip.Cursor = Cursors.Default;
        }

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

                if (datasets != null)
                {
                    datasetListComboBox.Items.Clear();

                    foreach (var obj in datasets)
                    {
                        datasetListComboBox.Items.Add(obj);
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
                datasetListComboBox.Enabled = true;
            }
        }

        private void datasetListComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            CUSTOM_UPLOADS_URI = rootCustomUploadsURI + datasetListComboBox.GetItemText(datasetListComboBox.SelectedItem) + "/";
            lblZipFile.Text = datasetListComboBox.GetItemText(datasetListComboBox.SelectedItem);
            btnUploadToS3.Enabled = false;
        }
    }
}
