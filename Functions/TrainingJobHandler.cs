using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSC_Trainer.Functions
{
    internal class TrainingJobHandler
    {
        private string prevStatusMessage = "";
        private string prevLogMessage = "";
        private int nextLogIndex = 0;
        private bool hasCustomUploads = false;
        private string datasetKey = "";
        private string s3Bucket = "";
        private bool deleting = false;
        private int delay = 0;

        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;
        private AmazonS3Client s3Client;

        private LSC_Trainer.Functions.IFileTransferUtility transferUtility;
        private Label instanceTypeBox;
        private Label trainingDurationBox;
        private Label trainingStatusBox;
        private Label descBox;
        private RichTextBox logBox;

        public TrainingJobHandler(AmazonSageMakerClient amazonSageMakerClient, AmazonCloudWatchLogsClient cloudWatchLogsClient, AmazonS3Client s3Client, Label instanceTypeBox, Label trainingDurationBox, Label trainingStatusBox, Label descBox, RichTextBox logBox, LSC_Trainer.Functions.IFileTransferUtility fileTransferUtility)
        {
            this.amazonSageMakerClient = amazonSageMakerClient;
            this.cloudWatchLogsClient = cloudWatchLogsClient;
            this.s3Client = s3Client;
            this.instanceTypeBox = instanceTypeBox;
            this.trainingDurationBox = trainingDurationBox;
            this.trainingStatusBox = trainingStatusBox;
            this.descBox = descBox;
            this.logBox = logBox;
            this.transferUtility = fileTransferUtility;
        }

        public async Task<bool> StartTrackingTrainingJob(string trainingJobName, string datasetKey, string s3Bucket, bool hasCustomUploads)
        {
            try
            {
                this.datasetKey = datasetKey;
                this.s3Bucket = s3Bucket;
                this.hasCustomUploads = hasCustomUploads;
                var completionSource = new TaskCompletionSource<bool>();
                var timer = InitializeTimer(trainingJobName, completionSource);
                timer.Start();

                if(await completionSource.Task)
                {
                    timer.Stop();
                };

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private System.Timers.Timer InitializeTimer(string trainingJobName, TaskCompletionSource<bool> completionSource)
        {
            // Create a Timer instance with a specified interval (e.g., every 5 secs)
            var timerInterval = 1000;
            var timer = new System.Timers.Timer(timerInterval);
            timer.Elapsed += async (sender, e) => await CheckTrainingJobStatus(amazonSageMakerClient, trainingJobName, completionSource);

            // The `CheckTrainingJobStatus` method will be called periodically based on the interval

            return timer;
        }
        private async Task CheckTrainingJobStatus(AmazonSageMakerClient amazonSageMakerClient, object state, TaskCompletionSource<bool> completionSource)
        {
            try {
                var trainingJobName = (string)state;

                // Retrieve the current status of the training job
                DescribeTrainingJobResponse trainingDetails = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
                {
                    TrainingJobName = trainingJobName
                });
                var trainingStatus = trainingDetails.TrainingJobStatus;

                TimeSpan timeSpan = TimeSpan.FromSeconds(trainingDetails.TrainingTimeInSeconds);
                string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                if (trainingStatus == TrainingJobStatus.InProgress)
                {
                    // Update training duration
                    if (trainingDetails.TrainingTimeInSeconds == 0)
                    {
                        UpdateTrainingStatus(
                            trainingDetails.ResourceConfig.InstanceType.ToString(),
                            formattedTime,
                            trainingDetails.SecondaryStatusTransitions.Last().Status,
                            trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                        );
                    }
                    else
                    {
                        UpdateTrainingStatus(formattedTime);
                    }

                    if (trainingDetails.SecondaryStatusTransitions.Any())
                    {
                        await CheckSecondaryStatus(trainingDetails, trainingJobName);
                    }
                }
                else if (trainingStatus == TrainingJobStatus.Completed)
                {
                    UpdateTrainingStatus(
                            trainingDetails.ResourceConfig.InstanceType.ToString(),
                            formattedTime,
                            trainingDetails.SecondaryStatusTransitions.Last().Status,
                            trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                    );

                    if (!completionSource.Task.IsCompleted) // Check if the TaskCompletionSource is already completed
                    {
                        completionSource.SetResult(true);
                        if (hasCustomUploads && !deleting)
                        {
                            deleting = true;
                            DisplayLogMessage($"{Environment.NewLine}Deleting dataset {datasetKey} from BUCKET ${s3Bucket}");
                            await transferUtility.DeleteDataSet(s3Client, s3Bucket, datasetKey);
                            DisplayLogMessage($"{Environment.NewLine}Dataset deletion complete.");
                        }
                        
                    }
                }
                else if (trainingStatus == TrainingJobStatus.Failed)
                {
                    DisplayLogMessage($"Training job failed: {trainingDetails.FailureReason}");
                    UpdateTrainingStatus(
                            trainingDetails.ResourceConfig.InstanceType.ToString(),
                            formattedTime,
                            trainingDetails.SecondaryStatusTransitions.Last().Status,
                            trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                    );

                    if (!completionSource.Task.IsCompleted) // Check if the TaskCompletionSource is already completed
                    {
                        completionSource.SetResult(true);

                    }
                }
                else
                {
                    DisplayLogMessage($"Training job stopped or in an unknown state.");
                    UpdateTrainingStatus(
                            trainingDetails.ResourceConfig.InstanceType.ToString(),
                            formattedTime,
                            trainingDetails.SecondaryStatusTransitions.Last().Status,
                            trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                    );

                    if (!completionSource.Task.IsCompleted) // Check if the TaskCompletionSource is already completed
                    {
                        completionSource.SetResult(true);
                        if (hasCustomUploads && !deleting)
                        {
                            deleting = true;
                            DisplayLogMessage($"{Environment.NewLine}Deleting dataset {datasetKey} from BUCKET ${s3Bucket}");
                            await transferUtility.DeleteDataSet(s3Client, s3Bucket, datasetKey);
                            DisplayLogMessage($"{Environment.NewLine}Dataset deletion complete.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the processing
                MessageBox.Show($"Error in Tracking Training Job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CheckSecondaryStatus(DescribeTrainingJobResponse trainingDetails, string trainingJobName)
        {
            //CloudWatch
            if (trainingDetails.SecondaryStatusTransitions.Any() && trainingDetails.SecondaryStatusTransitions.Last().Status == "Training")
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


                    if (logs.Events.Any() && prevLogMessage != logs.Events.Last().Message)
                    {
                        for (int i = nextLogIndex; i < logs.Events.Count; i++)
                        {
                            DisplayLogMessage(logs.Events[i].Message);
                        }
                        prevLogMessage = logs.Events.Last().Message;
                        nextLogIndex = logs.Events.IndexOf(logs.Events.Last()) + 1;
                    }
                }
                else
                {
                    delay++;
                    if (delay == 10 || delay == 0)
                    {
                        DisplayLogMessage($"{Environment.NewLine}Creating log stream for the training job.");
                        delay = 1;
                    }
                }
            }
            // Update training status
            if (trainingDetails.SecondaryStatusTransitions.Any() && trainingDetails.SecondaryStatusTransitions.Last().StatusMessage != prevStatusMessage)
            {
                UpdateTrainingStatus(
                    trainingDetails.SecondaryStatusTransitions.Last().Status,
                    trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                );
                prevStatusMessage = trainingDetails.SecondaryStatusTransitions.Last().StatusMessage;
            }
        }

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            Action updateUI = () =>
            {
                instanceTypeBox.Text = instanceType;
                trainingDurationBox.Text = trainingDuration;
                trainingStatusBox.Text = status;
                descBox.Text = description;
            };

            // Check if invoking is required
            if (instanceTypeBox.InvokeRequired || trainingDurationBox.InvokeRequired ||
                trainingStatusBox.InvokeRequired || descBox.InvokeRequired)
            {
                // Invoke on the UI thread
                instanceTypeBox.Invoke(updateUI);
                trainingDurationBox.Invoke(updateUI);
                trainingStatusBox.Invoke(updateUI);
                descBox.Invoke(updateUI);
            }
            else
            {
                // No invoke required, execute directly
                updateUI();
            }
        }

        public void UpdateTrainingStatus(string trainingDuration)
        {
            Action updateUI = () =>
            {
                trainingDurationBox.Text = trainingDuration;
            };

            // Check if invoking is required
            if (trainingDurationBox.InvokeRequired)
            {
                // Invoke on the UI thread
                trainingDurationBox.Invoke(updateUI);
            }
            else
            {
                // No invoke required, execute directly
                updateUI();
            }

        }

        public void UpdateTrainingStatus(string status, string description)
        {
            Action updateUI = () =>
            {
                trainingStatusBox.Text = status;
                descBox.Text = description;
            };
            if (trainingStatusBox.InvokeRequired || descBox.InvokeRequired)
            {
                // Invoke on the UI thread
                trainingStatusBox.Invoke(updateUI);
                descBox.Invoke(updateUI);
            }
            else
            {
                // No invoke required, execute directly
                updateUI();
            }
        }

        public static void DisplayLogMessage(string logMessage, RichTextBox logBox)
        {
            // Convert the ANSI log message to RTF
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
        public void DisplayLogMessage(string logMessage)
        {
            DisplayLogMessage(logMessage, logBox);
        }
        public static string ConvertAnsiToRtf(string ansiText)
        {
            ansiText = ansiText.Replace("#033[1m", @"\b ");
            ansiText = ansiText.Replace("#033[0m", @"\b0 ");
            ansiText = ansiText.Replace("#033[34m", @"\cf1 ");
            ansiText = ansiText.Replace("#033[0m", @"\cf0 ");
            ansiText = ansiText.Replace("#015", @"\line ");
            return @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}" + ansiText + "}";
        }

        public async Task<string> GetLatestLogStream(AmazonCloudWatchLogsClient amazonCloudWatchLogsClient, string logGroupName, string trainingJobName)
        {
            try {
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
            catch (Amazon.CloudWatchLogs.Model.ResourceNotFoundException ex)
            {
                // Log or handle the exception accordingly
                Console.WriteLine($"Log group '{logGroupName}' does not exist.");
                DisplayLogMessage($"{Environment.NewLine} The log group '{logGroupName}' is still being created or does not exist anymore.");
                return null;
            }catch(Exception ex)
            {
                // Log or handle the exception accordingly
                Console.WriteLine($"Error in getting log stream: {ex.Message}");
                DisplayLogMessage($"Error in getting log stream: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            // Dispose the resources
            amazonSageMakerClient.Dispose();
            cloudWatchLogsClient.Dispose();
            s3Client.Dispose();
        } 
    }
}
