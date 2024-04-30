using Amazon.CloudWatchLogs;
using Amazon.SageMaker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC_Trainer.Functions
{
    public interface ITrainingJobExecutor
    {
        CreateTrainingJobRequest CreateTrainingRequest(string img_size, string batch_size, string epochs, string weights, string data, string hyperparameters, string patience, string workers, string optimizer, string device, string instanceCount, string selectedInstance, string CUSTOM_UPLOADS_URI, string DEFAULT_DATASET_URI, string trainingFolder, string validationFolder, string ECR_URI, string SAGEMAKER_INPUT_DATA_PATH, string SAGEMAKER_OUTPUT_DATA_PATH, string ROLE_ARN, string DESTINATION_URI, string trainingJobName, CustomHyperParamsForm customHyperParamsForm);
        Task InitiateTrainingJob(CreateTrainingJobRequest trainingRequest, AmazonCloudWatchLogsClient cloudWatchLogsClient);
    }
}
