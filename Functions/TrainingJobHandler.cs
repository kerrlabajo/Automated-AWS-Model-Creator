using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime.EventStreams.Internal;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMaker.Model;
using Microsoft.Win32;
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
    /// <summary>
    /// Handles training job-related operations such as monitoring status, logging, and interacting with AWS services. Also responsible for
    /// updating the UI elements with the training job status and logs.
    /// </summary>
    internal class TrainingJobHandler
    {
        private string currentTrainingJobName;
        private bool hasCustomUploads = false;
        private string datasetKey = "";
        private string s3Bucket = "";
        private bool showDialogBox = false;

        private AmazonSageMakerClient amazonSageMakerClient;
        private AmazonCloudWatchLogsClient cloudWatchLogsClient;
        private AmazonS3Client s3Client;
        private StartLiveTailResponse startLiveTailResponse;

        private LSC_Trainer.Functions.IFileTransferUtility transferUtility;
        private IUIUpdater uIUpdater;

        private System.Timers.Timer timer;
       
        /// <summary> A CancellationTokenSource used to cancel the live tail session if needed.</param>
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingJobHandler"/> class with the specified Amazon SageMaker client, Amazon CloudWatch Logs client, Amazon S3 client, and UI elements for displaying training job information.
        /// </summary>
        /// <param name="amazonSageMakerClient">The Amazon SageMaker client to interact with SageMaker services.</param>
        /// <param name="cloudWatchLogsClient">The Amazon CloudWatch Logs client to interact with CloudWatch Logs.</param>
        /// <param name="s3Client">The Amazon S3 client to interact with S3 services.</param>
        public TrainingJobHandler(AmazonSageMakerClient amazonSageMakerClient, AmazonCloudWatchLogsClient cloudWatchLogsClient, AmazonS3Client s3Client, LSC_Trainer.Functions.IFileTransferUtility fileTransferUtility, IUIUpdater uIUpdater)
        {
            this.amazonSageMakerClient = amazonSageMakerClient;
            this.cloudWatchLogsClient = cloudWatchLogsClient;
            this.s3Client = s3Client;
            this.transferUtility = fileTransferUtility;
            this.uIUpdater = uIUpdater;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
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
                currentTrainingJobName = trainingJobName;
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

            return timer;
        }

        private readonly SemaphoreSlim startLiveTailLock = new SemaphoreSlim(1, 1);

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

        /// <summary>
        /// Asynchronously starts a live tail session for a CloudWatch log stream.
        /// Initializes a StartLiveTailRequest object and uses the CloudWatchLogs client to start the session.
        /// If successful, calls TrackLiveTail to begin tracking log events from the stream.
        /// 
        /// Assumes UIUpdater is an interface for updating the user interface with messages.
        /// Assumes TrackLiveTail is an asynchronous method that handles tracking log events from the live tail session.
        /// </summary>
        /// <param name="amazonCloudWatchLogsClient">An AmazonCloudWatchLogsClient instance used to interact with CloudWatch Logs.</param>
        /// <param name="logGroupName">The name of the CloudWatch log group containing the log stream.</param>
        /// <param name="logStreamName">The name of the log stream to tail for live updates.</param>
        /// <returns>An awaitable Task representing the asynchronous start live tail operation.</returns>
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

        /// <summary>
        /// Asynchronously tracks log events from a CloudWatch live tail session.
        /// Reads events from the response stream, processing different message types (session updates, messages, and errors).
        /// Handles cancellation and potential exceptions during the live tail session.
        /// 
        /// Assumes UIUpdater is an interface for updating the user interface with messages.
        /// Assumes lockObject is a synchronization object for thread-safe access to UI updates.
        /// </summary>
        /// <param name="response">A StartLiveTailResponse object containing the live tail session information and response stream.</param>
        /// <returns>An awaitable Task representing the asynchronous tracking operation.</returns>
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
                            if(uIUpdater != null)
                            {
                                uIUpdater.DisplayLogMessage("Live Tail session has ended.");
                            }
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

        /// <summary>
        /// Asynchronously retrieves details about a SageMaker training job.
        /// Initializes a DescribeTrainingJobRequest object with the training job name and uses the SageMaker client to get details.
        /// 
        /// Assumes the SageMaker client has been configured with proper credentials and region.
        /// </summary>
        /// <param name="amazonSageMakerClient">An AmazonSageMakerClient instance used to interact with SageMaker.</param>
        /// <param name="trainingJobName">The name of the SageMaker training job to get details for.</param>
        /// <returns>An awaitable Task that resolves to a DescribeTrainingJobResponse object containing the training job details.</returns>
        private async Task<DescribeTrainingJobResponse> GetTrainingJobDetails(AmazonSageMakerClient amazonSageMakerClient, string trainingJobName)
        {
            return await amazonSageMakerClient.DescribeTrainingJobAsync(new DescribeTrainingJobRequest
            {
                TrainingJobName = trainingJobName
            });
        }

        /// <summary>
        /// Asynchronously handles the deletion of custom uploaded datasets, prompting the user for confirmation.
        /// Checks if there are custom uploads and a confirmation dialog hasn't been shown yet.
        /// If so, displays a confirmation dialog asking the user if they want to delete the dataset from the S3 bucket.
        /// Based on the user's choice, deletes the dataset or displays a message skipping deletion.
        /// 
        /// Assumes UIUpdater is an interface for updating the user interface with messages.
        /// Assumes transferUtility is an object with a DeleteDataSet method for deleting datasets from S3.
        /// Assumes hasCustomUploads, showDialogBox, datasetKey, and s3Bucket are variables set elsewhere in the code.
        /// </summary>
        /// <returns>An awaitable Task representing the asynchronous deletion operation (if confirmed).</returns>
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

        /// <summary>
        /// Asynchronously retrieves the name of the latest log stream for a SageMaker training job.
        /// Uses the CloudWatchLogs client to describe log streams within the specified log group, searching for streams with a prefix matching the training job name.
        /// Returns the name of the latest log stream or null if none are found.
        /// 
        /// Assumes UIUpdater is an interface for updating the user interface with messages.
        /// </summary>
        /// <param name="amazonCloudWatchLogsClient">An AmazonCloudWatchLogsClient instance used to interact with CloudWatch Logs.</param>
        /// <param name="logGroupName">The name of the CloudWatch log group containing the training job logs.</param>
        /// <param name="trainingJobName">The name of the SageMaker training job to identify the associated log stream.</param>
        /// <returns>An awaitable Task that resolves to a string containing the name of the latest log stream or null if none are found.</returns>
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

        /// <summary>
        /// Event handler for the PowerModeChanged event, triggered when the system's power mode changes (e.g., going to sleep or waking up).
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments containing information about the power mode change.</param>
        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    // The system is going to sleep
                    // Pause or save your work here
                    cancellationTokenSource.Cancel();
                    break;
                case PowerModes.Resume:
                    // The system is waking up
                    // Resume your work here
                    cancellationTokenSource = new CancellationTokenSource();
                    string logGroupName = "/aws/sagemaker/TrainingJobs";
                    string logStreamName = await GetLatestLogStream(cloudWatchLogsClient, logGroupName, currentTrainingJobName);
                    if (!string.IsNullOrEmpty(logStreamName))
                    {
                        await StartLiveTail(cloudWatchLogsClient, $"arn:aws:logs:{UserConnectionInfo.Region}:{UserConnectionInfo.AccountId}:log-group:{logGroupName}:", logStreamName);
                    }
                    break;
            }
        }
        
        public void Dispose()
        {
            // Unsubscribe from system events
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;

            // Cancel the token to stop the task in TrackLiveTail
            cancellationTokenSource.Cancel();

            // Now that the task has completed, it's safe to dispose of the resources
            DisposeTimer(timer);
            amazonSageMakerClient.Dispose();
            cloudWatchLogsClient.Dispose();
            s3Client.Dispose();
            uIUpdater = null;
        }

        public void DisposeTimer(System.Timers.Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}
