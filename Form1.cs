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
        
        public Form1()
        {
            InitializeComponent();

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
        }

        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///TODO: To test if your AWS credentials is accessible and able to connect, execute this event with the AmazonSageMakerClient.
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

        private void btnUploadToS3_Click(object sender, EventArgs e)
        {
            if(datasetPath != null)
            {
                string filename = datasetPath.Split('\\').Last();
                DialogResult result = MessageBox.Show($"Do you want to upload {filename} to s3 bucket?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) 
                {
                    //byte[] fileByteArray = File.ReadAllBytes(datasetPath);
                    if (isFile)
                    {
                        //string zipKey =  AWS_Helper.UploadFileToS3(s3Client, datasetPath, filename, uploadBucketName);
                        //string zipKey = "CITU_Dataset-2023-12-11-00-1233.rar";
                        Task.Run(async () => await AWS_Helper.UnzipAndUploadToS3(s3Client, uploadBucketName, datasetPath)).Wait();
                    }
                    else
                    {
                        Task.Run(async () => await AWS_Helper.UploadFolderToS3(s3Client, datasetPath, filename, uploadBucketName)).Wait();
                    }
                   
                }
            }
            else
            {
                MessageBox.Show("No file to upload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }
        private void btnTraining_Click(object sender, EventArgs e)
        {
            string img_num = "";
            string img_size = "1280";
            string weights = "yolov5n6.pt";
            string patience = "100";
            string hyperparameters = "hyp.no-augmentation.yaml";
            string epochs = "1";
            string batch_size = "1";
            string project = "";
            string workers = "8";
            string optimiser = "SGD";

            if(txtImgNum.Text != null || txtImgNum.Text != "")
            {
                img_num = txtImgNum.Text;
            }

            if (txtImgSize.Text != null || txtImgSize.Text != "")
            {
                img_size = txtImgSize.Text;
            }

            if (txtWeights.Text != null || txtWeights.Text != "")
            {
                weights = txtWeights.Text;
            }

            if (txtPatience.Text != null || txtPatience.Text != "")
            {
                patience = txtPatience.Text;
            }

            if (txtHyperparameters.Text != null || txtHyperparameters.Text != "")
            {
                hyperparameters = txtHyperparameters.Text;
            }

            if (txtEpochs.Text != null || txtEpochs.Text != "")
            {
                epochs = txtEpochs.Text;
            }

            if (txtBatchSize.Text != null || txtBatchSize.Text != "")
            {
                batch_size = txtBatchSize.Text;
            }

            if (txtProject.Text != null || txtProject.Text != "")
            {
                project = txtProject.Text;
            }

            if (txtWorkers.Text != null || txtWorkers.Text != "")
            {
                workers = txtWorkers.Text;
            }

            if (txtOptimiser.Text != null || txtOptimiser.Text != "")
            {
                optimiser = txtOptimiser.Text;
            }

            string jobName = String.Format("Training-YOLOv5-UbuntuCUDAIMG-{0}", DateTime.Now.ToString("yyyy-MM-dd-hh-mmss"));

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
                        "--data", sageMakerInputDataPath + "train/MMX059XA_COVERED5B.yaml",
                        "--hyp", hyperparameters,
                        "--project", sageMakerOutputDataPath,
                        "--name", name,
                    }
                },
                RoleArn = roleARN,
                OutputDataConfig = new OutputDataConfig()
                {
                    S3OutputPath = s3DestinationURI + "output/",
                },
                ResourceConfig = new ResourceConfig()
                {
                    InstanceCount = 1,
                    InstanceType = TrainingInstanceType.MlM5Xlarge,
                    VolumeSizeInGB = 12
                },
                TrainingJobName = jobName,
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
                                S3Uri = s3DatasetURI + "train",
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
                                S3Uri = s3DatasetURI + "Verification Images",
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
                
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            datasetPath = null;
            lblZipFile.Text = "No file selected";
            btnRemoveFile.Visible = false;
        }

        private void btnDownloadModel_Click(object sender, EventArgs e)
        {
            //temporary 
            string modelKey = "weights/23070a53best.onnx";
            //modelKey = modelKey.Replace($"s3://{bucketName}", "");

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

        ///TODO: Create a button to upload a dataset in .rar/.zip file.

        ///TODO: Create a text area to get the training parameters:
        ///TODO: number of images to train [NOT YET SURE/IMPLEMENTED IN YOLOV5 REPO]
        ///TODO: image size (default 1280), batch size, epochs, 
        ///TODO: weights (default yolov5n6.pt), project (default S3 destination), 
        ///TODO: patience (default 100), workers (default 8), optimizer (default SGD), 
        ///TODO: hyperparameters (default hyp.scratch-low.yamL)

        ///TODO: Create a text area to print the ff:
        ///TODO: 1. AmazonSageMakerClient connection duration.
        ///TODO: 2. Dataset file size, and uploading network connectivity and duration
        ///TODO: 3. Virtual machine specs used for training (the instance we selected to create the training job).
        ///TODO: 4. Duration of the training in AWS SageMaker Training Job creation and training.
        ///TODO: 5. Model url that was saved in AWS S3.

        ///TODO: Create a proceed training button to create a training job in AWS.
        ///TODO: Implement the create training job using SageMaker with the .env variable that contains the AWS ECR location 
        ///TODO: of the team's training algorithm, specific EC2 Instance (Virtual Machine) that is free version, and other
        ///TODO: specific configurations in the web interface that can be replicated here in AWSSDK.SageMaker.

    }
}
