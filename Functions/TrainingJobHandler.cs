using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.EC2;
using Amazon.ECR.Model;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LSC_Trainer.Functions
{
    /// <summary>
    /// Handles training job-related operations such as monitoring status, logging, and interacting with AWS services. Also responsible for
    /// updating the UI elements with the training job status and logs.
    /// </summary>
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

        private Label instanceTypeBox;
        private Label trainingDurationBox;
        private Label trainingStatusBox;
        private Label descBox;
        private RichTextBox logBox;

        /// <summary>
        /// Initializes a new instance of the TrainingJobHandler class with the specified Amazon SageMaker client, Amazon CloudWatch Logs client, Amazon S3 client, and UI elements for displaying training job information.
        /// </summary>
        /// <param name="amazonSageMakerClient">The Amazon SageMaker client to interact with SageMaker services.</param>
        /// <param name="cloudWatchLogsClient">The Amazon CloudWatch Logs client to interact with CloudWatch Logs.</param>
        /// <param name="s3Client">The Amazon S3 client to interact with S3 services.</param>
        /// <param name="instanceTypeBox">The label control to display the instance type of the training job.</param>
        /// <param name="trainingDurationBox">The label control to display the duration of the training job.</param>
        /// <param name="trainingStatusBox">The label control to display the status of the training job.</param>
        /// <param name="descBox">The label control to display the description of the training job.</param>
        /// <param name="logBox">The RichTextBox control to display logs related to the training job.</param>
        public TrainingJobHandler(AmazonSageMakerClient amazonSageMakerClient, AmazonCloudWatchLogsClient cloudWatchLogsClient, AmazonS3Client s3Client, Label instanceTypeBox, Label trainingDurationBox, Label trainingStatusBox, Label descBox, RichTextBox logBox)
        {
            this.amazonSageMakerClient = amazonSageMakerClient;
            this.cloudWatchLogsClient = cloudWatchLogsClient;
            this.s3Client = s3Client;
            this.instanceTypeBox = instanceTypeBox;
            this.trainingDurationBox = trainingDurationBox;
            this.trainingStatusBox = trainingStatusBox;
            this.descBox = descBox;
            this.logBox = logBox;
        }

        /// <summary>
        /// Initiates tracking of the specified training job, updating UI elements with its progress and status.
        /// </summary>
        /// <param name="trainingJobName">The name of the training job to track.</param>
        /// <param name="datasetKey">The key of the dataset associated with the training job.</param>
        /// <param name="s3Bucket">The name of the S3 bucket containing the training job data.</param>
        /// <param name="hasCustomUploads">A boolean indicating whether the training job uses custom uploads.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result indicates whether the tracking of the training job was successful.
        /// </returns>
        /// <exception cref="Exception">Thrown when an error occurs during the tracking of the training job.</exception>
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

        /// <summary>
        /// Initializes a timer to periodically check the status of the specified training job.
        /// </summary>
        /// <param name="trainingJobName">The name of the training job to monitor.</param>
        /// <param name="completionSource">The TaskCompletionSource used to signal completion of the tracking operation.</param>
        /// <returns>The initialized System.Timers.Timer instance.</returns>
        private System.Timers.Timer InitializeTimer(string trainingJobName, TaskCompletionSource<bool> completionSource)
        {
            // Create a Timer instance with a specified interval (e.g., every 5 secs)
            var timerInterval = 1000;
            var timer = new System.Timers.Timer(timerInterval);
            timer.Elapsed += async (sender, e) => await CheckTrainingJobStatus(amazonSageMakerClient, trainingJobName, completionSource);

            // The `CheckTrainingJobStatus` method will be called periodically based on the interval

            return timer;
        }

        /// <summary>
        /// Asynchronously checks the status of a training job and updates the UI accordingly.
        /// </summary>
        /// <param name="amazonSageMakerClient">The Amazon SageMaker client used to interact with training jobs.</param>
        /// <param name="state">The state object containing the training job name.</param>
        /// <param name="completionSource">The TaskCompletionSource used to signal completion of the tracking operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the tracking of the training job.</exception>
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
                            await AWS_Helper.DeleteDataSet(s3Client, s3Bucket, datasetKey);
                            DisplayLogMessage($"{Environment.NewLine}Dataset deletion complete.");
                        }
                        
                    }
                }
                else if (trainingStatus == TrainingJobStatus.Failed)
                {
                    DisplayLogMessage($"Training job failed: {trainingDetails.FailureReason}");
                }
                else
                {
                    DisplayLogMessage($"Training job stopped or in an unknown state.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the processing
                MessageBox.Show($"Error in Tracking Training Job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Asynchronously checks the secondary status of a training job and updates the UI with status messages.
        /// This function is also responsible for logging the training job progress using CloudWatch logs.
        /// </summary>
        /// <param name="trainingDetails">The response containing details of the training job.</param>
        /// <param name="trainingJobName">The name of the training job.</param>
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

        /// <summary>
        /// Updates the training status label with the provided instance type, training duration, status, and description.
        /// </summary>
        /// <param name="instanceType">The instance type used for training.</param>
        /// <param name="trainingDuration">The duration of the training.</param>
        /// <param name="status">The status of the training job.</param>
        /// <param name="description">A description of the training job status.</param>
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

        /// <summary>
        /// Updates the training duration label with the provided training duration value.
        /// </summary>
        /// <param name="trainingDuration">The duration of the training.</param>
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

        /// <summary>
        /// Updates the training status and description labels with the provided values.
        /// </summary>
        /// <param name="status">The status of the training.</param>
        /// <param name="description">The description of the training.</param>
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

        /// <summary>
        /// Displays the provided log message in the specified RichTextBox control.
        /// </summary>
        /// <param name="logMessage">The log message to be displayed.</param>
        /// <param name="logBox">The RichTextBox control where the log message will be displayed.</param>
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
        /// <summary>
        /// Displays the provided log message in the associated RichTextBox control.
        /// </summary>
        /// <param name="logMessage">The log message to be displayed.</param>
        public void DisplayLogMessage(string logMessage)
        {
            DisplayLogMessage(logMessage, logBox);
        }

        /// <summary>
        /// Converts ANSI formatted text to RTF format for a more readable display in a RichTextBox control.
        /// </summary>
        /// <param name="ansiText">The ANSI formatted text to be converted.</param>
        /// <returns>The RTF formatted string.</returns>
        public static string ConvertAnsiToRtf(string ansiText)
        {
            ansiText = ansiText.Replace("#033[1m", @"\b ");
            ansiText = ansiText.Replace("#033[0m", @"\b0 ");
            ansiText = ansiText.Replace("#033[34m", @"\cf1 ");
            ansiText = ansiText.Replace("#033[0m", @"\cf0 ");
            ansiText = ansiText.Replace("#015", @"\line ");
            return @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}" + ansiText + "}";
        }

        /// <summary>
        /// Retrieves the latest log stream associated with a given log group and training job name.
        /// </summary>
        /// <param name="amazonCloudWatchLogsClient">The Amazon CloudWatch Logs client instance.</param>
        /// <param name="logGroupName">The name of the log group.</param>
        /// <param name="trainingJobName">The name of the training job.</param>
        /// <returns>The name of the latest log stream, or null if not found.</returns>
        /// <exception cref="Amazon.CloudWatchLogs.Model.ResourceNotFoundException"> Thrown when the log group does not exist.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while retrieving the log stream.</exception>
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
    }
}
