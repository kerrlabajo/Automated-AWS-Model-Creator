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
        public CreateTrainingJobRequest CreateTrainingRequest(string img_size, string batch_size, string epochs, string weights, string data, string hyperparameters, string patience, string workers, string optimizer, string device, string instanceCount, string selectedInstance,string CUSTOM_UPLOADS_URI, string DEFAULT_DATASET_URI, string trainingFolder, string validationFolder, string ECR_URI, string SAGEMAKER_INPUT_DATA_PATH, string SAGEMAKER_OUTPUT_DATA_PATH, string ROLE_ARN, string DESTINATION_URI, string trainingJobName, CustomHyperParamsForm customHyperParamsForm)
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
                        "--data", SAGEMAKER_INPUT_DATA_PATH + data,
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
                                S3Uri = (utility.HasCustomUploads(CUSTOM_UPLOADS_URI) ? CUSTOM_UPLOADS_URI : DEFAULT_DATASET_URI) + trainingFolder,
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
                                S3Uri = (utility.HasCustomUploads(CUSTOM_UPLOADS_URI) ? CUSTOM_UPLOADS_URI : DEFAULT_DATASET_URI) + validationFolder,
                                S3DataDistributionType = S3DataDistribution.FullyReplicated
                            }
                        }
                    }
                }
            };
            return trainingRequest;
        }

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
