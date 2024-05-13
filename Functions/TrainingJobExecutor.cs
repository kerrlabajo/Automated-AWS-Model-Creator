using Amazon.CloudWatchLogs;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC_Trainer.Functions
{
    public class TrainingJobExecutor : ITrainingJobExecutor
    {
        private Utility utility = new Utility();
        private IUIUpdater uiUpdater;
        private TrainingJobHandler trainingJobHandler;


        public TrainingJobExecutor(IUIUpdater uiUpdater)
        {
            this.uiUpdater = uiUpdater;
        }

        /// <summary>
        /// Creates a `CreateTrainingJobRequest` for starting a training job with the specified parameters.
        /// </summary>
        /// <param name="img_size">The size of the images.</param>
        /// <param name="batch_size">The batch size for training.</param>
        /// <param name="epochs">The number of epochs for training.</param>
        /// <param name="weights">The weights for initialization.</param>
        /// <param name="data">The data used for training.</param>
        /// <param name="hyperparameters">The hyperparameters for training.</param>
        /// <param name="patience">The patience parameter for training.</param>
        /// <param name="workers">The number of workers used for training.</param>
        /// <param name="optimizer">The optimizer used for training.</param>
        /// <param name="device">The device used for training.</param>
        /// <param name="instanceCount">The number of instances to use for training.</param>
        /// <param name="selectedInstance">The type of instance selected for training.</param>
        /// <param name="CUSTOM_UPLOADS_URI">The URI for custom uploads.</param>
        /// <param name="DEFAULT_DATASET_URI">The default dataset URI.</param>
        /// <param name="ECR_URI">The ECR URI.</param>
        /// <param name="SAGEMAKER_INPUT_DATA_PATH">The input data path for SageMaker.</param>
        /// <param name="SAGEMAKER_OUTPUT_DATA_PATH">The output data path for SageMaker.</param>
        /// <param name="ROLE_ARN">The ARN of the role used for training.</param>
        /// <param name="DESTINATION_URI">The URI for the output data.</param>
        /// <param name="trainingJobName">The name of the training job.</param>
        /// <param name="customHyperParamsForm">The form containing custom hyperparameters.</param>
        /// <returns>A `CreateTrainingJobRequest` for starting a training job.</returns>
        public CreateTrainingJobRequest CreateTrainingRequest(string img_size, string batch_size, string epochs, string weights, string data, string hyperparameters, string patience, string workers, string optimizer, string device, string instanceCount, string selectedInstance,string CUSTOM_UPLOADS_URI, string DEFAULT_DATASET_URI, string ECR_URI, string SAGEMAKER_INPUT_DATA_PATH, string SAGEMAKER_OUTPUT_DATA_PATH, string ROLE_ARN, string DESTINATION_URI, string trainingJobName, CustomHyperParamsForm customHyperParamsForm)
        {
            CreateTrainingJobRequest trainingRequest = new CreateTrainingJobRequest()
            {
                AlgorithmSpecification = new AlgorithmSpecification()
                {
                    TrainingInputMode = "File",
                    TrainingImage = ECR_URI,
                    ContainerEntrypoint = new List<string>() { "python3", "/code/train_and_export.py" },
                    ContainerArguments = new List<string>()
                    {
                        "--img-size", img_size,
                        "--batch", batch_size,
                        "--epochs", epochs,
                        "--weights", weights,
                        "--data", SAGEMAKER_INPUT_DATA_PATH + data + $"/{data}.yaml",
                        "--hyp", hyperparameters,
                        "--project", SAGEMAKER_OUTPUT_DATA_PATH,
                        "--name", "results",
                        "--patience", patience,
                        "--workers", workers,
                        "--optimizer", optimizer,
                        "--device", device,
                        "--include", "onnx",
                        "--nnodes", instanceCount
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
                    InstanceCount = int.Parse(instanceCount),
                    // Update the instance type everytime you select an instance type
                    InstanceType = TrainingInstanceType.FindValue(selectedInstance),
                    VolumeSizeInGB = 50
                },
                TrainingJobName = trainingJobName,
                StoppingCondition = new StoppingCondition()
                {
                    MaxRuntimeInSeconds = 86400,
                    MaxWaitTimeInSeconds = 86400,
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
                        ChannelName = data,
                        InputMode = TrainingInputMode.File,
                        CompressionType = Amazon.SageMaker.CompressionType.None,
                        RecordWrapperType = RecordWrapper.None,
                        DataSource = new DataSource()
                        {
                            S3DataSource = new S3DataSource()
                            {
                                S3DataType = S3DataType.S3Prefix,
                                S3Uri = (utility.HasCustomUploads(CUSTOM_UPLOADS_URI) ? CUSTOM_UPLOADS_URI : DEFAULT_DATASET_URI),
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    },
                }
            };
            return trainingRequest;
        }

        /// <summary>
        /// Initiates a training job using the specified training request and sets up tracking for the job.
        /// </summary>
        /// <param name="trainingRequest">The training request containing parameters for the training job.</param>
        /// <param name="cloudWatchLogsClient">The CloudWatch Logs client.</param>
        /// <param name="amazonSageMakerClient">The Amazon SageMaker client.</param>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="transferUtility">The file transfer utility for S3 operations.</param>
        /// <param name="datasetKey">The key of the dataset in S3.</param>
        /// <param name="bucket">The bucket name where the dataset is stored.</param>
        /// <param name="HasCustomUploads">A flag indicating if custom uploads are used.</param>
        public async Task InitiateTrainingJob(CreateTrainingJobRequest trainingRequest, AmazonCloudWatchLogsClient cloudWatchLogsClient, AmazonSageMakerClient amazonSageMakerClient, AmazonS3Client s3Client, LSC_Trainer.Functions.IFileTransferUtility transferUtility, string datasetKey, string bucket, bool HasCustomUploads)
        {
            CreateTrainingJobResponse response = amazonSageMakerClient.CreateTrainingJob(trainingRequest);
            string trainingJobName = response.TrainingJobArn.Split(':').Last().Split('/').Last();
            

            uiUpdater.SetLogPanelVisibility(true);
            trainingJobHandler = new TrainingJobHandler(amazonSageMakerClient, cloudWatchLogsClient, s3Client, transferUtility, uiUpdater);

            bool success = await trainingJobHandler.StartTrackingTrainingJob(trainingJobName, datasetKey, bucket, HasCustomUploads);
        }

        public void Dispose()
        {
             trainingJobHandler?.Dispose();
 
        }
        public async Task<CreateTrainingJobResponse> CreateTrainingJobResponse(CreateTrainingJobRequest trainingRequest, AmazonSageMakerClient amazonSageMakerClient)
        {
            return await amazonSageMakerClient.CreateTrainingJobAsync(trainingRequest);
        }
    }
}
