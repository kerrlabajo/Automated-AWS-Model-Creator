using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using Amazon.SageMakerRuntime;
using System.Linq;
using Amazon.S3.Model;
using LSC_Trainer.Functions;
using System.Threading.Tasks;

namespace LSC_Trainer
{
    public partial class Form1 : Form
    {
        private delegate void SetProgressCallback(int percentDone);
        private readonly AmazonSageMakerClient amazonSageMakerClient;
        private readonly AmazonS3Client s3Client;
        private readonly AmazonSageMakerRuntimeClient amazonSageMakerRuntimeClient;
        private readonly string s3URI;
        private readonly string s3UploadURI;
        private readonly string ecrURI;
        private readonly string dockerImageURI;
        private readonly string s3DatasetURI;
        private readonly string s3DestinationURI;
        private readonly string sageMakerInputDataPath = "/opt/ml/input/data/";
        private readonly string sageMakerOutputDataPath = "/opt/ml/output/data/";
        private readonly string sageMakerModelPath = "/opt/ml/model/";
        private readonly string roleARN;
        private readonly string bucketName;
        private readonly string uploadBucketName;

        private string datasetPath;
        private bool isFile;

        private string training_folder;
        private string validation_folder;
        
        private string filename;
        //temporary solution (need to figure out how to implement when there are multiple training jobs)
        private string trainingJobName;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .env").Replace("\\","/");
            
            DotNetEnv.Env.Load(fullPath);

            string accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
            string secretKey = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
            string region = Environment.GetEnvironmentVariable("REGION");
            s3URI = Environment.GetEnvironmentVariable("S3_URI");
            s3UploadURI = Environment.GetEnvironmentVariable("S3_BUCKET_UPLOADS_URI");
            ecrURI = Environment.GetEnvironmentVariable("ECR_URI");
            dockerImageURI = Environment.GetEnvironmentVariable("DOCKER_IMAGE_URI");
            s3DatasetURI = Environment.GetEnvironmentVariable("S3_DATASET_URI");
            s3DestinationURI = Environment.GetEnvironmentVariable("S3_DESTINATION_URI");
            roleARN = Environment.GetEnvironmentVariable("ARN");

            amazonSageMakerClient = new AmazonSageMakerClient(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));

            uploadBucketName = s3UploadURI.Replace("s3://", "");
            uploadBucketName = uploadBucketName.Replace("/", "");

            bucketName = s3URI.Replace("s3://", "");
            bucketName = bucketName.Replace("/", "");

            string datasetName = s3DatasetURI.Split('/').Reverse().Skip(1).First();
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
                // txtDevice.Text = "0";
                training_folder = "train";
                validation_folder = "Verification Images";
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
                // txtDevice.Text = "0";
                training_folder = "train";
                validation_folder = "val";
            }
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
                }
            }
        }

        private void btnUploadToS3_Click(object sender, EventArgs e)
        {
            if(datasetPath != null)
            {
                filename = datasetPath.Split('\\').Last();
                DialogResult result = MessageBox.Show($"Do you want to upload {filename} to s3 bucket?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) 
                {
                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("No file to upload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void btnTraining_Click(object sender, EventArgs e)
        {
            string img_size = "";
            string batch_size = "";
            string epochs = "";
            string weights = "";
            string data = "";
            string hyperparameters = "";
            string patience = "";
            string workers = "";
            string optimizer = "";
            string device = "";

            if (txtImageSize.Text != "")
            {
                img_size = txtImageSize.Text;
            }

            if (txtBatchSize.Text != "")
            {
                batch_size = txtBatchSize.Text;
            }

            if (txtEpochs.Text != "")
            {
                epochs = txtEpochs.Text;
            }

            if (txtWeights.Text != "")
            {
                weights = txtWeights.Text;
            }

            if (txtData.Text != "")
            {
                data = txtData.Text;
            }

            if (txtHyperparameters.Text != "")
            {
                hyperparameters = txtHyperparameters.Text;
            }

            if (txtPatience.Text != "")
            {
                patience = txtPatience.Text;
            }

            if (txtWorkers.Text != "")
            {
                workers = txtWorkers.Text;
            }

            if (txtOptimizer.Text != "")
            {
                optimizer = txtOptimizer.Text;
            }

            if (txtDevice.Text != "")
            {
                device = txtDevice.Text;
            }

             trainingJobName = String.Format("Training-YOLOv5-UbuntuCUDAIMG-{0}", DateTime.Now.ToString("yyyy-MM-dd-hh-mmss"));

            CreateTrainingJobRequest trainingRequest = new CreateTrainingJobRequest()
            {
                AlgorithmSpecification = new AlgorithmSpecification()
                {
                    TrainingInputMode = "File",
                    TrainingImage = ecrURI,
                    ContainerEntrypoint = new List<string>() { "python3", "yolov5/train.py" },
                    ContainerArguments = new List<string>()
                    {
                        "--img-size", img_size,
                        "--batch", batch_size,
                        "--epochs", epochs,
                        "--weights", weights,
                        "--data", sageMakerInputDataPath + "train/" + data,
                        "--hyp", hyperparameters,
                        "--project", sageMakerOutputDataPath,
                        "--name", "results",
                        "--patience", patience,
                        "--workers", workers,
                        "--optimizer", optimizer,
                        // "--device", device
                    }
                },
                RoleArn = roleARN,
                OutputDataConfig = new OutputDataConfig()
                {
                    S3OutputPath = s3DestinationURI
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
                                S3Uri = s3DatasetURI + training_folder,
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
                                S3Uri = s3DatasetURI + validation_folder,
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    }
                }             
            };

            try
            {
                CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
                string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();
                // Process the response if needed

                Console.WriteLine("Training job executed successfully.");

                string prevStatusMessage = "";
                Timer timer = new Timer();
                timer.Interval = 5000; // Check every 5 seconds
                timer.Tick += async (sender1, e1) => {
                    try
                    {
                        // Get the training job status
                        DescribeTrainingJobResponse tracker = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
                        {
                            TrainingJobName = trainingJobName
                        });

                        //SecondaryStatusTransition status = tracker.SecondaryStatusTransitions.Last();
                        //Console.WriteLine("Status: " + status.Status);
                        //Console.WriteLine("Description: " + status.StatusMessage);
                         
                        if (tracker.SecondaryStatusTransitions.Last().StatusMessage != prevStatusMessage)
                        {
                            Console.WriteLine($"Status: { tracker.SecondaryStatusTransitions.Last().Status}");
                            Console.WriteLine($"Description: {tracker.SecondaryStatusTransitions.Last().StatusMessage}");
                            Console.WriteLine();
                            prevStatusMessage = tracker.SecondaryStatusTransitions.Last().StatusMessage;
                            // Update the UI with the latest status
                            // UpdateUi(response.TrainingJobStatus); to be implemented
                        }

                        if (tracker.TrainingJobStatus == TrainingJobStatus.Completed)
                        {
                            Console.WriteLine("Printing status history...");
                            foreach(SecondaryStatusTransition history in tracker.SecondaryStatusTransitions)
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
                            timer.Stop(); // Stop timer when training is complete
                        }

                        if(tracker.TrainingJobStatus == TrainingJobStatus.Failed)
                        {
                            Console.WriteLine(tracker.FailureReason);
                            timer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors
                        MessageBox.Show($"Error in training model: {ex.Message}");
                    }
                };

                timer.Start(); // Start the timer
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating training job: {ex.Message}");
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            datasetPath = null;
            lblZipFile.Text = "No file selected";
            btnRemoveFile.Visible = false;
        }

        private void btnDownloadModel_Click(object sender, EventArgs e)
        {
            string modelKey = $"{trainingJobName}/output/<output.tar.gz>";

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
    {
                folderBrowserDialog.Description = "Select a folder to save the file";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string saveFolder = folderBrowserDialog.SelectedPath;

                    DialogResult result = MessageBox.Show($"Do you want to save the model to {saveFolder} ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Task.Run(async() => await AWS_Helper.DownloadFile(s3Client, bucketName, modelKey, saveFolder)).Wait();
                    }
                }
            }
        }
                }
            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (isFile)
            {
                AWS_Helper.UnzipAndUploadToS3(s3Client, uploadBucketName, datasetPath, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
            }
            else
            {
                AWS_Helper.UploadFolderToS3(s3Client, datasetPath, filename, uploadBucketName, new Progress<int>(percent =>
                {
                    backgroundWorker.ReportProgress(percent);
                })).Wait();
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Upload completed!");
        }

        private void SelectAllTextOnClick(object sender, EventArgs e)
        {
            sender.GetType().GetMethod("SelectAll")?.Invoke(sender, null);
        }

        private void btnUnzipGZ_Click(object sender, EventArgs e)
        {
            //remove first output after update
            //string modelKey = $"output/{trainingJobName}/output/output.tar.gz";
            string modelKey = $"output/Training-YOLOv5-UbuntuCUDAIMG-2023-12-13-11-0446/output/output.tar.gz";


            //DialogResult result = MessageBox.Show($"Do you want to save the model to {saveFolder} ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            Task.Run(async () => await AWS_Helper.ExtractAndUploadBestPt(s3Client, bucketName, modelKey)).Wait();
               
            
        }
    }
}
