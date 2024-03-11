using Amazon.CloudWatchLogs;
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

        public void StartTrackingTrainingJob(string trainingJobName, bool hasCustomUploads)
        {
            instanceTypeBox.Text = "test";
        }

        private void TrackTrainingJob(string trainingJobName, bool hasCustomUploads, System.Windows.Forms.Timer timer)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in training model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //private System.Windows.Forms.Timer InitializeTimer()
        //{
        //    // Create a Timer instance with a specified interval (e.g., every 5 secs)
        //    var timerInterval = 5000;
        //    //var timer = new Timer(CheckTrainingJobStatus, trainingJobName, TimeSpan.Zero, timerInterval);

        //    // The `CheckTrainingJobStatus` method will be called periodically based on the interval

        //    return timer;
        //}
        private async Task CheckTrainingJobStatus(AmazonSageMakerClient amazonSageMakerClient, object state)
        {
            var trainingJobName = (string)state;

            // Retrieve the current status of the training job
            DescribeTrainingJobResponse trainingDetails = await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
            {
                TrainingJobName = trainingJobName
            });
            var trainingStatus = trainingDetails.TrainingJobStatus;

            if(trainingStatus == TrainingJobStatus.InProgress)
            {
                // Update training duration
                TimeSpan timeSpan = TimeSpan.FromSeconds(trainingDetails.TrainingTimeInSeconds);
                string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

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
                CheckSecondaryStatus(trainingDetails);

            }
            else if(trainingStatus == TrainingJobStatus.Completed)
            {

            }else if(trainingStatus == TrainingJobStatus.Failed)
            {

            }
            else
            {

            }
        }

        private void CheckSecondaryStatus(DescribeTrainingJobResponse trainingDetails)
        {
            
        }

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            instanceTypeBox.Text = instanceType;
            trainingDurationBox.Text = trainingDuration;
            trainingStatusBox.Text = status;
            descBox.Text = description;
        }

        public void UpdateTrainingStatus(string trainingDuration)
        {
            trainingDurationBox.Text = trainingDuration;
        }

        public void DisplayLogMessage(string logMessage)
        {
            // Convert the ANSI log message to RTF
            string rtfMessage = ConvertAnsiToRtf(logMessage);

            // Remove the start and end of the RTF document from the message
            rtfMessage = rtfMessage.Substring(rtfMessage.IndexOf('}') + 1);
            rtfMessage = rtfMessage.Substring(0, rtfMessage.LastIndexOf('}'));

            // Append the RTF message at the end of the existing RTF text
            logBox.Rtf = logBox.Rtf.Insert(logBox.Rtf.LastIndexOf('}'), rtfMessage);

            // Scroll to the end to show the latest log messages
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
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
    }
}
