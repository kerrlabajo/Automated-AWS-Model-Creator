using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.EC2;
using Amazon.ECR.Model;
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
    internal class TrainingJobHandler
    {
        private string prevStatusMessage = "";
        private string prevLogMessage = "";
        private int prevLogIndex = 0;

        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;

        private Label instanceTypeBox;
        private Label trainingDurationBox;
        private Label trainingStatusBox;
        private Label descBox;
        private RichTextBox logBox;

        public TrainingJobHandler(AmazonSageMakerClient amazonSageMakerClient, AmazonCloudWatchLogsClient cloudWatchLogsClient, Label instanceTypeBox, Label trainingDurationBox, Label trainingStatusBox, Label descBox, RichTextBox logBox)
        {
            this.amazonSageMakerClient = amazonSageMakerClient;
            this.cloudWatchLogsClient = cloudWatchLogsClient;
            this.instanceTypeBox = instanceTypeBox;
            this.trainingDurationBox = trainingDurationBox;
            this.trainingStatusBox = trainingStatusBox;
            this.descBox = descBox;
            this.logBox = logBox;
        }

        public async Task<bool> StartTrackingTrainingJob(string trainingJobName, bool hasCustomUploads)
        {
            try
            {
                var completionSource = new TaskCompletionSource<bool>();
                var timer = InitializeTimer(trainingJobName, completionSource);
                timer.Start();

                await completionSource.Task;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //private void TrackTrainingJob(string trainingJobName, bool hasCustomUploads, System.Windows.Forms.Timer timer)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //}

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

                if (trainingStatus == TrainingJobStatus.InProgress)
                {
                    // Update training duration
                    TimeSpan timeSpan = TimeSpan.FromSeconds(trainingDetails.TrainingTimeInSeconds);
                    string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                    if (trainingDetails.TrainingTimeInSeconds == 0)
                    {
                        await UpdateTrainingStatus(
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
                    await CheckSecondaryStatus(trainingDetails, trainingJobName);

                }
                else if (trainingStatus == TrainingJobStatus.Completed)
                {
                    if (!completionSource.Task.IsCompleted) // Check if the TaskCompletionSource is already completed
                    {
                        completionSource.SetResult(true);
                    }
                }
                else if (trainingStatus == TrainingJobStatus.Failed)
                {
                    DisplayLogMessage($"Training job failed: {trainingDetails.FailureReason}");
                    //btnTraining.Enabled = true;
                    //timer.Stop();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the processing
                completionSource.SetException(ex);
            }
        }

        private async Task CheckSecondaryStatus(DescribeTrainingJobResponse trainingDetails, string trainingJobName)
        {
            //CloudWatch
            if (trainingDetails.SecondaryStatusTransitions.Last().Status == "Training")
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


                    if (prevLogMessage != logs.Events.Last().Message)
                    {
                        for (int i = prevLogIndex + 1; i < logs.Events.Count; i++)
                        {
                            DisplayLogMessage(logs.Events[i].Message);
                        }
                        prevLogMessage = logs.Events.Last().Message;
                        prevLogIndex = logs.Events.IndexOf(logs.Events.Last());
                    }
                }
            }
            // Update training status
            if (trainingDetails.SecondaryStatusTransitions.Last().StatusMessage != prevStatusMessage)
            {
                UpdateTrainingStatus(
                    trainingDetails.SecondaryStatusTransitions.Last().Status,
                    trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                );
                prevStatusMessage = trainingDetails.SecondaryStatusTransitions.Last().StatusMessage;
            }
        }

        public async Task UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
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

        public void DisplayLogMessage(string logMessage)
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

        public string ConvertAnsiToRtf(string ansiText)
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
    }
}
