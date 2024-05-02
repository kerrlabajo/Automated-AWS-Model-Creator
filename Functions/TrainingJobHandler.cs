using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime.EventStreams.Internal;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
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
        private bool showDialogBox = false;
        private int delay = 0;

        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;
        private AmazonS3Client s3Client;
        private StartLiveTailResponse startLiveTailResponse;

        private LSC_Trainer.Functions.IFileTransferUtility transferUtility;
        private IUIUpdater uIUpdater;

        private System.Timers.Timer timer;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public TrainingJobHandler(AmazonSageMakerClient amazonSageMakerClient, AmazonCloudWatchLogsClient cloudWatchLogsClient, AmazonS3Client s3Client, LSC_Trainer.Functions.IFileTransferUtility fileTransferUtility, IUIUpdater uIUpdater)
        {
            this.amazonSageMakerClient = amazonSageMakerClient;
            this.cloudWatchLogsClient = cloudWatchLogsClient;
            this.s3Client = s3Client;
            this.transferUtility = fileTransferUtility;
            this.uIUpdater = uIUpdater;
        }

        public async Task<bool> StartTrackingTrainingJob(string trainingJobName, string datasetKey, string s3Bucket, bool hasCustomUploads)
        {
            try
            {
                this.datasetKey = datasetKey;
                this.s3Bucket = s3Bucket;
                this.hasCustomUploads = hasCustomUploads;
                var completionSource = new TaskCompletionSource<bool>();
                timer = InitializeTimer(trainingJobName, completionSource);
                timer.Start();

                if(await completionSource.Task)
                {
                    timer.Stop();
                    await HandleCustomUploads();
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

            return timer;
        }

        private readonly SemaphoreSlim startLiveTailLock = new SemaphoreSlim(1, 1);

        private async Task CheckTrainingJobStatus(AmazonSageMakerClient amazonSageMakerClient, object state, TaskCompletionSource<bool> completionSource)
        {
            try
            {
                var trainingJobName = (string)state;
                DescribeTrainingJobResponse trainingDetails = await GetTrainingJobDetails(amazonSageMakerClient, trainingJobName);
                var trainingStatus = trainingDetails.TrainingJobStatus;

                TimeSpan timeSpan = TimeSpan.FromSeconds(trainingDetails.TrainingTimeInSeconds);
                string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                if (trainingStatus == TrainingJobStatus.InProgress)
                {
                    uIUpdater.UpdateTrainingStatus(
                        trainingDetails.ResourceConfig.InstanceType.ToString(),
                        formattedTime,
                        trainingDetails.SecondaryStatusTransitions.Last().Status,
                        trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                    );

                    await startLiveTailLock.WaitAsync();
                    try
                    {
                        if (startLiveTailResponse == null)
                        {
                            if (trainingDetails.SecondaryStatusTransitions.Any() && trainingDetails.SecondaryStatusTransitions.Last().Status == "Training")
                            {
                                string logStreamName = await GetLatestLogStream(cloudWatchLogsClient, "/aws/sagemaker/TrainingJobs", trainingJobName);
                                if (!string.IsNullOrEmpty(logStreamName))
                                {
                                    await StartLiveTail(cloudWatchLogsClient, "arn:aws:logs:ap-southeast-1:905418164808:log-group:/aws/sagemaker/TrainingJobs:", logStreamName);
                                }
                                    
                            }
                        }
                    }
                    finally
                    {
                        startLiveTailLock.Release();
                    }

                }
                else if(trainingStatus == TrainingJobStatus.Completed)
                {
                    if (!completionSource.Task.IsCompleted)
                    {
                        completionSource.SetResult(true);
                    }
                }
                else if(trainingStatus == TrainingJobStatus.Failed)
                {
                    uIUpdater.DisplayLogMessage($"Training job failed: {trainingDetails.FailureReason}");
                    if (!completionSource.Task.IsCompleted)
                    {
                        completionSource.SetResult(true);
                    }
                }
                else
                {
                    uIUpdater.DisplayLogMessage($"Training job stopped or in an unknown state.");
                    if (!completionSource.Task.IsCompleted)
                    {
                        completionSource.SetResult(true);
                    }
                }

                if (completionSource.Task.IsCompleted){
                    cancellationTokenSource.Cancel();
                    uIUpdater.UpdateTrainingStatus(
                        trainingDetails.ResourceConfig.InstanceType.ToString(),
                        formattedTime,
                        trainingDetails.SecondaryStatusTransitions.Last().Status,
                        trainingDetails.SecondaryStatusTransitions.Last().StatusMessage
                    );
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in Tracking Training Job: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task StartLiveTail(AmazonCloudWatchLogsClient amazonCloudWatchLogsClient, string logGroupName, string logStreamName)
        {
            var response = await amazonCloudWatchLogsClient.StartLiveTailAsync(new StartLiveTailRequest
            {
                LogGroupIdentifiers = new List<string>() { logGroupName },
                LogStreamNames = new List<string>() { logStreamName },
            }, cancellationTokenSource.Token);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                uIUpdater.DisplayLogMessage("Failed to start live tail session");
                return ;
            }
            startLiveTailResponse = response;
            TrackLiveTail(startLiveTailResponse);
        }

        private readonly object lockObject = new object();
        private async void TrackLiveTail(StartLiveTailResponse response)
        {
            var eventStream = response.ResponseStream;
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), cancellationTokenSource.Token);

            try
            {
                await Task.Run(() =>
                {
                    foreach (var item in eventStream)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            eventStream.Dispose();
                            uIUpdater.DisplayLogMessage("Live Tail session has ended.");
                            break;
                        }
                        lock (lockObject)
                        {
                            if (item is LiveTailSessionUpdate liveTailSessionUpdate)
                            {
                                foreach (var sessionResult in liveTailSessionUpdate.SessionResults)
                                {
                                    uIUpdater.DisplayLogMessage(sessionResult.Message);
                                }
                            }
                            if (item is LiveTailSessionStart)
                            {
                                uIUpdater.DisplayLogMessage("Live Tail session started");
                            }
                            // On-stream exceptions are processed here
                            if (item is CloudWatchLogsEventStreamException)
                            {
                                uIUpdater.DisplayLogMessage($"ERROR: {item}");
                            }
                        }
                        
                    }
                }, cancellationTokenSource.Token);
            }catch (OperationCanceledException)
            {
                uIUpdater.DisplayLogMessage("Live Tail session has been cancelled.");
            }
            catch (CloudWatchLogsEventStreamException ex)
            {
                Console.WriteLine($"ERROR: {ex}");
            }
            catch (IOException ex)
            {
                // Handle IOException here
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            catch (SocketException ex)
            {
                // Handle SocketException here
                Console.WriteLine($"ERROR: {ex.Message}");

            }
        }

        private async Task<DescribeTrainingJobResponse> GetTrainingJobDetails(AmazonSageMakerClient amazonSageMakerClient, string trainingJobName)
        {
            return await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
            {
                TrainingJobName = trainingJobName
            });
        }

        private async Task HandleCustomUploads()
        {
            if (hasCustomUploads && !showDialogBox)
            {
                showDialogBox = true;

                if(MessageBox.Show("Do you want to delete the dataset from the S3 bucket?", "Delete Dataset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    uIUpdater.DisplayLogMessage($"{Environment.NewLine}Deleting dataset {datasetKey} from BUCKET ${s3Bucket}");
                    await transferUtility.DeleteDataSet(s3Client, s3Bucket, datasetKey);
                    uIUpdater.DisplayLogMessage($"{Environment.NewLine}Dataset deletion complete.");
                }
                else
                {
                    uIUpdater.DisplayLogMessage($"{Environment.NewLine}Skipping dataset deletion.");
                }
            }
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
            catch (Amazon.CloudWatchLogs.Model.ResourceNotFoundException)
            {
                // Log or handle the exception accordingly
                Console.WriteLine($"Log group '{logGroupName}' does not exist.");
                uIUpdater.DisplayLogMessage($"{Environment.NewLine} The log group '{logGroupName}' is still being created or does not exist anymore.");
                return null;
            }catch(Exception ex)
            {
                // Log or handle the exception accordingly
                Console.WriteLine($"Error in getting log stream: {ex.Message}");
                uIUpdater.DisplayLogMessage($"Error in getting log stream: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            // Cancel the token to stop the task in TrackLiveTail
            cancellationTokenSource.Cancel();

            // Now that the task has completed, it's safe to dispose of the resources
            DisposeTimer(timer);
            amazonSageMakerClient.Dispose();
            cloudWatchLogsClient.Dispose();
            s3Client.Dispose();
        }

        public void DisposeTimer(System.Timers.Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}
