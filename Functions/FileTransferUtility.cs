using Amazon.S3.Transfer;
using Amazon.S3;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.Windows.Forms;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Threading;
using System.Net;
using Amazon.Runtime;

namespace LSC_Trainer.Functions
{
    internal class FileTransferUtility : IFileTransferUtility
    {
        private long totalUploaded = 0;

        private int  overallPercentage = 0;
        private IUIUpdater UIUpdater { get; set; }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private bool isMessageBoxShown = false;
        public FileTransferUtility(IUIUpdater uIUpdater)
        {
            UIUpdater = uIUpdater;
        }

        /// <summary>
        /// Asynchronously unpacks a local ZIP file and uploads the extracted files to a specified Amazon S3 bucket and folder.
        /// Reports upload progress through the provided IProgress instance and allows cancellation using a CancellationTokenSource.
        /// Logs upload time and potential errors.
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="bucketName">The name of the S3 bucket to upload files to.</param>
        /// <param name="folder">The folder within the bucket to upload the extracted files to.</param>
        /// <param name="localZipFilePath">The path to the local ZIP file to unpack and upload.</param>
        /// <param name="progress">An IProgress instance for reporting upload progress (percentage).</param>
        /// <returns>An awaitable Task representing the asynchronous upload operation.</returns>
        public async Task UnzipAndUploadToS3(AmazonS3Client s3Client, string bucketName, string folder, string localZipFilePath, IProgress<int> progress)
        {
            try
            {
                totalUploaded = 0;
                overallPercentage = 0;
                cancellationTokenSource = new CancellationTokenSource();
                DateTime startTime = DateTime.Now;
                long totalSize = CalculateTotalSize(localZipFilePath);

                using (var fileStream = File.OpenRead(localZipFilePath))
                {
                    using (var zipStream = new ZipInputStream(fileStream))
                    {
                        await ProcessZipEntries(s3Client, zipStream, bucketName, folder, progress, totalSize);
                    }
                }

                Console.WriteLine("Successfully uploaded all files from the folder to S3.");
                LogUploadTime(startTime);
            }
            catch (AmazonS3Exception e)
            {
                LogError("Error uploading zipfile to S3 here: ", e);
            }
            catch (Exception e)
            {
                LogError("Error uploading zipfile to S3: ", e);
            }
        }

        /// <summary>
        /// Uploads a file to Amazon S3.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client instance.</param>
        /// <param name="filePath">The local file path of the file to upload.</param>
        /// <param name="fileName">The name of the file to be stored in S3.</param>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="progress">An instance of IProgress to report the progress of the upload.</param>
        /// <param name="totalSize">The total size of the file being uploaded.</param>
        /// <returns>The name of the file uploaded to S3 if successful, otherwise null.</returns>
        public async Task<string> UploadFileToS3(AmazonS3Client s3Client, string filePath, string fileName, string bucketName, IProgress<int> progress, long totalSize, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
                DateTime startTime = DateTime.Now;

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    var uploadRequest = CreateUploadRequest(filePath, fileName, bucketName);
                    ConfigureProgressTracking(uploadRequest, progress, totalSize, UIUpdater, cancellationTokenSource.Token);

                    await transferUtility.UploadAsync(uploadRequest, cancellationTokenSource.Token);

                    if (!cancellationToken.IsCancellationRequested && UIUpdater != null)
                    {
                        UIUpdater.UpdateTrainingStatus($"Uploading Files to S3", $"Uploading {totalUploaded}/{totalSize} - {overallPercentage}%");
                    }
                }


                LogUploadTime(startTime);
                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                if (e.ErrorCode == "RequestTimeTooSkewed")
                {
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error uploading file to S3: A file took too long to upload. The difference between the request time and the current time is too large.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;   
                    }
                    else
                    {
                        Console.WriteLine($"Error uploading file to S3: A file took too long to upload. The difference between the request time and the current time is too large.");
                    }
                }
                else
                {
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error uploading file to S3: {e}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;
                    }
                    else
                    {
                        LogError("Error uploading file to S3: ", e);
                    }
                }
                cancellationTokenSource.Cancel();
                return null;
            }
            catch (AmazonServiceException e)
            {
                if (e.InnerException is WebException webEx && webEx.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    // Handle the NameResolutionFailure exception
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error in Tracking Training Job: Failed to resolve the hostname. Please check your network connection and the hostname.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;
                    }
                    else
                    {
                        Console.WriteLine($"Error in Tracking Training Job: Failed to resolve the hostname. Please check your network connection and the hostname.");
                    }
                }
                else
                {
                    LogError("Error uploading file to S3: An error occurred within the AWS SDK.", e);
                }
                cancellationTokenSource.Cancel();
                return null;
            }
            catch (OperationCanceledException e)
            {
                LogError("File Upload has been cancelled: ", e);
                return null;
            }
            catch (Exception e)
            {
                if (!isMessageBoxShown)
                {
                    isMessageBoxShown = true;
                    MessageBox.Show($"Error uploading file to S3: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isMessageBoxShown = false;
                }
                else
                {
                    LogError("Error uploading file to S3: ", e);
                }
                cancellationTokenSource.Cancel();
                return null;
            }
        }

        /// <summary>
        /// Uploads a file from a MemoryStream to Amazon S3.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client instance.</param>
        /// <param name="fileStream">The MemoryStream containing the file data.</param>
        /// <param name="fileName">The name of the file to be stored in S3.</param>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="progress">An instance of IProgress to report the progress of the upload.</param>
        /// <param name="totalSize">The total size of the file being uploaded.</param>
        /// <returns>The name of the file uploaded to S3 if successful, otherwise null.</returns>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>"
        public async Task<string> UploadFileToS3(AmazonS3Client s3Client, MemoryStream fileStream, string fileName, string bucketName, IProgress<int> progress, long totalSize, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
                DateTime startTime = DateTime.Now;
                    
                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    var uploadRequest = CreateUploadRequest(fileStream, fileName, bucketName);
                    ConfigureProgressTracking(uploadRequest, progress, totalSize, UIUpdater, cancellationTokenSource.Token);

                    await transferUtility.UploadAsync(uploadRequest, cancellationTokenSource.Token);

                    if (!cancellationToken.IsCancellationRequested && UIUpdater!=null)
                    {
                        UIUpdater.UpdateTrainingStatus($"Uploading Files to S3", $"Uploading {totalUploaded}/{totalSize} - {overallPercentage}%");
                    }
                }
                

                LogUploadTime(startTime);
                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                if (e.ErrorCode == "RequestTimeTooSkewed")
                {
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error uploading file to S3: A file took too long to upload. The difference between the request time and the current time is too large.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;
                    }
                    else
                    {
                        Console.WriteLine($"Error uploading file to S3: A file took too long to upload. The difference between the request time and the current time is too large.");
                    }
                }
                else
                {
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error uploading file to S3: {e}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;
                    }
                    else
                    {
                        LogError("Error uploading file to S3: ", e);
                    }                   
                }
                cancellationTokenSource.Cancel();
                return null;
            }
            catch (AmazonServiceException e)
            {
                if (e.InnerException is WebException webEx && webEx.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    // Handle the NameResolutionFailure exception
                    if (!isMessageBoxShown)
                    {
                        isMessageBoxShown = true;
                        MessageBox.Show($"Error in uploading file to S3: Failed to resolve the hostname. Please check your network connection and the hostname.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isMessageBoxShown = false;
                    }
                    else
                    {
                        Console.WriteLine($"Error in uploading file to S3: Failed to resolve the hostname. Please check your network connection and the hostname.");
                    }
                }
                else
                {
                    LogError("Error uploading file to S3: An error occurred within the AWS SDK.", e);
                }
                cancellationTokenSource.Cancel();
                return null;
            }
            catch (OperationCanceledException e)
            {
                LogError("File Upload has been cancelled: ", e);
                return null;
            }
            catch (Exception e)
            {
                if (!isMessageBoxShown)
                {
                    isMessageBoxShown = true;
                    MessageBox.Show($"Error uploading file to S3: {e}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isMessageBoxShown = false;
                }
                else
                {
                    LogError("Error uploading file to S3: ", e);
                }
                cancellationTokenSource.Cancel();
                return null;
            }
        }

        /// <summary>
        /// Gets the contents of a folder and uploads them to Amazon S3 asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client instance.</param>
        /// <param name="bucketName">The name of the S3 bucket where the files will be uploaded.</param>
        /// <param name="localZipFilePath">The local path of the ZIP file to extract and upload.</param>
        /// <param name="progress">An instance of IProgress to report the progress of the upload.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public async Task UploadFolderToS3(AmazonS3Client s3Client, string folderPath, string folderName, string bucketName, IProgress<int> progress)
        {
            try
            {
                overallPercentage = 0;
                totalUploaded = 0;
                cancellationTokenSource = new CancellationTokenSource();
                DateTime startTime = DateTime.Now;
                long totalSize = CalculateTotalSizeFolder(folderPath);
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

                // Limit the number of concurrent uploads
                var semaphore = new SemaphoreSlim(10); // Change this number to the maximum number of concurrent uploads you want

                var tasks = files.Select(async file =>
                {
                    await semaphore.WaitAsync();

                    try
                    {
                        var key = GenerateKey(folderPath, file, folderName);
                        await UploadFileToS3(s3Client, file, key, bucketName, progress, totalSize, cancellationTokenSource.Token);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);

                Console.WriteLine("Successfully uploaded all files from the folder to S3.");
                LogUploadTime(startTime);
            }
            catch (AmazonS3Exception e)
            {
                LogError("Error uploading folder to S3: ", e);
            }
            catch (Exception e)
            {
                LogError("Error uploading folder to S3: ", e);
            }
        }

        /// <summary>
        /// Extracts a Tar GZip (TAR.GZ) archive file to a specified local folder.
        /// Skips directories within the archive and overwrites existing files by default.
        /// </summary>
        /// <param name="tarFilePath">The path to the TAR.GZ archive file to extract.</param>
        /// <param name="localFilePath">The base path of the local folder to extract files to (files will be written with their full paths within the archive preserved).</param>
        public static void ExtractTarGz(string tarFilePath, string localFilePath)
        {
            using (Stream stream = File.OpenRead(tarFilePath))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        ExtractionOptions opt = new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        };
                        reader.WriteEntryToDirectory(localFilePath, opt);
                    }
                }
            }
        }

        /// <summary>
        /// Downloads an object/file from an Amazon S3 bucket to a local file asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client used to perform the download operation.</param>
        /// <param name="bucketName">The name of the Amazon S3 bucket containing the object/file to download.</param>
        /// <param name="objectKey">The key of the object/file to download from the Amazon S3 bucket.</param>
        /// <param name="localFilePath">The local file path where the downloaded object/file will be saved.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a string indicating the status of the download operation.</returns>
        /// <exception cref="AggregateException">Thrown when an exception is encountered during the download operation.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when access to the local file system is denied.</exception>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        public async Task<string> DownloadObjects(AmazonS3Client s3Client, string bucketName, string objectKey, string localFilePath)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                DateTime startTime = DateTime.Now;
                string filePath = PrepareLocalFile(localFilePath, objectKey);

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityDownloadRequest downloadRequest = CreateDownloadRequest(bucketName, objectKey, filePath);

                    ConfigureProgressTracking(downloadRequest, UIUpdater, cancellationTokenSource.Token);

                    await transferUtility.DownloadAsync(downloadRequest, cancellationTokenSource.Token);
                }
                string tarGzPath = Path.Combine(localFilePath, objectKey.Split('/').Last());
                ExtractTarGz(tarGzPath, localFilePath);

                // Delete the TAR.GZ file after extraction
                File.Delete(tarGzPath);

                // Delete PaxHeaders.X directories
                var paxHeaderDirectories = Directory.EnumerateDirectories(localFilePath, "PaxHeaders.*", SearchOption.AllDirectories);
                foreach (var directory in paxHeaderDirectories)
                {
                    Directory.Delete(directory, true);
                }

                return GenerateResponseMessage(startTime, filePath);
            }
            catch (AggregateException e)
            {
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
            catch (AmazonS3Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Asynchronously deletes a dataset (file) identified by its key from a specified Amazon S3 bucket.
        /// First lists potential objects with the provided key prefix and then deletes them in a batch.
        /// Displays a success message on deletion and logs potential errors.
        /// 
        /// Throws the following exceptions:
        ///  - ArgumentNullException:
        ///     - If `s3Client` is null.
        ///     - If `bucketName` is null or empty.
        ///     - If `key` is null or empty.
        ///  - AggregateException: Thrown when an exception is encountered during the deletion operation (e.g., network issues, errors deleting multiple objects).
        ///  - UnauthorizedAccessException: Thrown when access to the local file system is denied (assuming this might be relevant during success message display using MessageBox.Show).
        ///  - AmazonS3Exception: Thrown when an error occurs while interacting with Amazon S3 services (e.g., permission problems, object not found).
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="bucketName">The name of the S3 bucket containing the dataset.</param>
        /// <param name="key">The key (filename) of the dataset to delete (including any path within the bucket).</param>
        /// <returns>An awaitable Task representing the asynchronous deletion operation.</returns>
        public async Task DeleteDataSet(AmazonS3Client s3Client, string bucketName, string key)
        {
            try
            {
                ListObjectsV2Request listRequest = CreateListRequest(bucketName, key);

                await DeleteObjectsInList(s3Client, bucketName, listRequest);

                MessageBox.Show($"Deleted dataset: {key}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error deleting objects from S3: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting objects from S3: " + e.Message);
            }
        }

        /// <summary>
        /// Creates a TransferUtilityUploadRequest object for uploading a file to Amazon S3.
        /// Sets the bucket name, object key (filename in S3), file path, and content type based on the filename extension.
        /// 
        /// Assumes GetContentType is a helper function that maps filename extensions to MIME content types.
        /// </summary>
        /// <param name="filePath">The path to the local file to upload.</param>
        /// <param name="fileName">The desired filename for the object in the S3 bucket.</param>
        /// <param name="bucketName">The name of the S3 bucket to upload the file to.</param>
        /// <returns>A TransferUtilityUploadRequest object configured for the upload.</returns>
        private static TransferUtilityUploadRequest CreateUploadRequest(string filePath, string fileName, string bucketName)
        {
            return new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = filePath,
                ContentType = GetContentType(fileName)
            };
        }

        /// <summary>
        /// Creates a TransferUtilityUploadRequest object for uploading a file from a MemoryStream to Amazon S3.
        /// Sets the bucket name, object key (filename in S3), input stream (MemoryStream), and content type based on the filename extension.
        /// 
        /// Assumes GetContentType is a helper function that maps filename extensions to MIME content types.
        /// 
        /// This overload is useful for uploading file data that's already in memory as a stream.
        /// </summary>
        /// <param name="fileStream">The MemoryStream containing the file data to upload.</param>
        /// <param name="fileName">The desired filename for the object in the S3 bucket.</param>
        /// <param name="bucketName">The name of the S3 bucket to upload the file to.</param>
        /// <returns>A TransferUtilityUploadRequest object configured for the upload.</returns>
        private static TransferUtilityUploadRequest CreateUploadRequest(MemoryStream fileStream, string fileName, string bucketName)
        {
            return new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = fileStream,
                ContentType = GetContentType(fileName)
            };
        }

        /// <summary>
        /// Configures progress tracking for a TransferUtility upload request.
        /// Attaches an event handler to the UploadProgressEvent to report upload progress 
        /// through the provided IProgress instance and update the UI using the IUIUpdater.
        /// 
        /// Tracks total uploaded bytes and calculates overall upload percentage based on total size.
        /// 
        /// Assumes IProgress and IUIUpdater are interfaces for reporting progress and updating UI respectively.
        /// </summary>
        /// <param name="uploadRequest">The TransferUtilityUploadRequest object to configure.</param>
        /// <param name="progress">An IProgress instance for reporting upload progress (percentage).</param>
        /// <param name="totalSize">The total size of the file being uploaded.</param>
        /// <param name="UIUpdater">An IUIUpdater instance for updating the user interface with progress.</param>
        /// <param name="cancellationToken">A CancellationToken used to cancel the upload if needed.</param>
        private void ConfigureProgressTracking(TransferUtilityUploadRequest uploadRequest, IProgress<int> progress, long totalSize, IUIUpdater UIUpdater, CancellationToken cancellationToken)
        {

            uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
            {
                long currentFileSize = args.TransferredBytes;
                
                if(args.PercentDone == 100)
                {
                    totalUploaded += currentFileSize;
                    overallPercentage = (int)(totalUploaded * 100 / totalSize);
                    progress.Report(overallPercentage);
                }
            });
        }

        /// <summary>
        /// Configures progress tracking for a TransferUtility download request.
        /// Attaches an event handler to the WriteObjectProgressEvent to update the UI with download progress 
        /// using the provided IUIUpdater, but only if cancellation is not requested.
        /// 
        /// Shows downloaded bytes, total bytes, and percentage within the UI updater.
        /// 
        /// Assumes IUIUpdater is an interface for updating the user interface with progress.
        /// </summary>
        /// <param name="downloadRequest">The TransferUtilityDownloadRequest object to configure.</param>
        /// <param name="UIUpdater">An IUIUpdater instance for updating the user interface with progress.</param>
        /// <param name="cancellationToken">A CancellationToken used to cancel the download if needed.</param>
        private static void ConfigureProgressTracking(TransferUtilityDownloadRequest downloadRequest, IUIUpdater UIUpdater, CancellationToken cancellationToken)
        {

            downloadRequest.WriteObjectProgressEvent += (sender, args) =>
            {
                int percentage = args.PercentDone;
                if (!cancellationToken.IsCancellationRequested)
                {
                    UIUpdater.UpdateTrainingStatus($"Downloading Files from S3", $"Downloading {args.TransferredBytes}/{args.TotalBytes} - {percentage}%");

                    UIUpdater.UpdateDownloadStatus(percentage);
                }
                
            };
        }

        /// <summary>
        /// Logs the total upload time to the console in a human-readable format (hours:minutes:seconds.milliseconds).
        /// </summary>
        /// <param name="startTime">The DateTime object representing the start time of the upload process.</param>
        private static void LogUploadTime(DateTime startTime)
        {
            TimeSpan totalTime = DateTime.Now - startTime;
            string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                          (int)totalTime.TotalHours,
                                          totalTime.Minutes,
                                          totalTime.Seconds,
                                          (int)(totalTime.Milliseconds / 100));
            Console.WriteLine($"Upload time: {formattedTotalTime}");
        }

        /// <summary>
        /// Logs an error message to the console, including the provided message and the exception's message.
        /// </summary>
        /// <param name="message">A message describing the error context.</param>
        /// <param name="e">The Exception object containing the error details.</param>
        private static void LogError(string message, Exception e)
        {
            Console.WriteLine($"{message} {e.Message}");
        }

        /// <summary>
        /// Generates a key (filename) for storing an object in Amazon S3 based on the provided file path and folder structure.
        /// 
        /// Assumes PathHelper.GetRelativePath is a helper function that gets the relative path between two paths.
        /// </summary>
        /// <param name="folderPath">The base path of the folder containing the file.</param>
        /// <param name="file">The filename of the file to upload.</param>
        /// <param name="folderName">The desired folder name within S3 to store the object.</param>
        /// <returns>The generated key (filename) for the S3 object.</returns>
        private static string GenerateKey(string folderPath, string file, string folderName)
        {
            var relativePath = PathHelper.GetRelativePath(folderPath, file);
            var key = relativePath.Replace(Path.DirectorySeparatorChar, '/');
            return folderName + "/" + key;
        }

        /// <summary>
        /// Asynchronously processes entries from a ZipInputStream and uploads them to Amazon S3.
        /// Iterates through entries in the zip stream, decompresses non-directory entries to a MemoryStream,
        /// and uploads them to S3 using UploadFileToS3 with progress reporting and cancellation support.
        /// 
        /// Assumes DecompressEntryAsync is an asynchronous method that decompresses a ZipEntry to a stream.
        /// Assumes UploadFileToS3 is an asynchronous method that uploads a file to S3.
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="zipStream">The ZipInputStream containing the zip archive data.</param>
        /// <param name="bucketName">The name of the S3 bucket to upload the files to.</param>
        /// <param name="folder">The desired folder name within the S3 bucket to store the uploaded files (optional).</param>
        /// <param name="progress">An IProgress instance for reporting upload progress (percentage).</param>
        /// <param name="totalSize">The total size of the zip archive (used for progress calculation).</param>
        /// <param name="cancellationTokenSource">A CancellationTokenSource used to cancel the upload process if needed.</param>
        /// <returns>An awaitable Task representing the asynchronous processing operation.</returns>
        private async Task ProcessZipEntries(AmazonS3Client s3Client, ZipInputStream zipStream, string bucketName, string folder, IProgress<int> progress, long totalSize)
        {
            ZipEntry entry;
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                if (!entry.IsDirectory)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await DecompressEntryAsync(zipStream, memoryStream);

                        string fileName = folder + entry.Name;

                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        await UploadFileToS3(s3Client, memoryStream, fileName, bucketName, progress, totalSize, cancellationTokenSource.Token);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the total size of all files within a directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path of the directory.</param>
        /// <returns>The total size of all files within the directory and its subdirectories, in bytes.</returns>   
        public static long CalculateTotalSizeFolder(string directoryPath)
        {
            Console.WriteLine($"Filename - {directoryPath}");
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            return dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        /// <summary>
        /// Calculates the total size of all files within a zip file.
        /// </summary>
        /// <param name="directoryPath">The path of the zip file.</param>
        /// <returns>The total size of all entries within the zip archive, in bytes.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified zip file is not found.</exception>
        public static long CalculateTotalSize(string directoryPath)
        {
            long totalSize = 0;
            if (!File.Exists(directoryPath))
            {
                throw new FileNotFoundException("The zip file was not found.", directoryPath);
            }

            using (ZipArchive archive = System.IO.Compression.ZipFile.OpenRead(directoryPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    totalSize += entry.Length;
                }
            }

            return totalSize;
        }

        /// <summary>
        /// Asynchronously decompresses data from a zip stream into a memory stream.
        /// </summary>
        /// <param name="zipStream">The zip input stream containing compressed data.</param>
        /// <param name="memoryStream">The memory stream to write the decompressed data to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task DecompressEntryAsync(ZipInputStream zipStream, MemoryStream memoryStream)
        {
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Determines the content type of a file based on its extension.
        /// </summary>
        /// <param name="fileName">The name of the file with its extension.</param>
        /// <returns>
        /// The content type corresponding to the file extension. If the extension is not recognized,
        /// returns "application/octet-stream", which is the default content type for unknown file types.
        /// </returns>
        private static string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            switch (extension)
            {
                case ".txt":
                    return "text/plain";
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                case ".yaml":
                    return "application/x-yaml";
                case ".xml":
                    return "application/xml";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".bmp":
                    return "image/bmp";
                case ".csv":
                    return "text/csv";
                case ".zip":
                    return "application/zip";
                case ".rar":
                    return "application/x-rar-compressed";
                default:
                    return "application/octet-stream";
            }
        }

        /// <summary>
        /// Prepares a local file path for downloading an object from Amazon S3.
        /// Ensures the directory path exists and constructs the local file path with the object key's last element (filename).
        /// 
        /// This method is useful for handling situations where the object key in S3 might contain a path structure.
        /// 
        /// Assumes the object key uses forward slashes ('/') as path separators.
        /// </summary>
        /// <param name="localFilePath">The desired local file path for the downloaded object (including filename).</param>
        /// <param name="objectKey">The key (filename) of the object in S3, potentially containing a path structure.</param>
        /// <returns>The prepared local file path for downloading the object.</returns>
        private static string PrepareLocalFile(string localFilePath, string objectKey)
        {
            string directoryPath = Path.GetDirectoryName(localFilePath);
            Directory.CreateDirectory(directoryPath);
            return Path.Combine(localFilePath, objectKey.Split('/').Last());
        }

        /// <summary>
        /// Creates a TransferUtilityDownloadRequest object for downloading a file from Amazon S3.
        /// Sets the bucket name, object key (filename in S3), and local file path for the download.
        /// </summary>
        /// <param name="bucketName">The name of the S3 bucket containing the object to download.</param>
        /// <param name="objectKey">The key (filename) of the object in S3.</param>
        /// <param name="filePath">The local file path where the downloaded object will be saved.</param>
        /// <returns>A TransferUtilityDownloadRequest object configured for the download.</returns>
        private static TransferUtilityDownloadRequest CreateDownloadRequest(string bucketName, string objectKey, string filePath)
        {
            return new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                FilePath = filePath
            };
        }

        /// <summary>
        /// Generates a response message summarizing a successful download operation.
        /// 
        /// Includes the download location (file path) and total download time in a human-readable format.
        /// </summary>
        /// <param name="startTime">The DateTime object representing the start time of the download process.</param>
        /// <param name="filePath">The local file path where the downloaded object was saved.</param>
        /// <returns>The formatted response message string.</returns>
        private static string GenerateResponseMessage(DateTime startTime, string filePath)
        {
            string response = $"\n File has been saved to {filePath} \n";
            TimeSpan totalTime = DateTime.Now - startTime;
            string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                (int)totalTime.TotalHours,
                totalTime.Minutes,
                totalTime.Seconds,
                (int)(totalTime.Milliseconds / 100));
            response += $"Total Time Taken: {formattedTotalTime}";
            return response;
        }

        /// <summary>
        /// Creates a ListObjectsV2Request object to list objects in a specific folder within an Amazon S3 bucket.
        /// Sets the bucket name and a prefix (folder path) to filter the listed objects.
        /// 
        /// Assumes you want to list objects within a particular folder identified by the "key" parameter.
        /// </summary>
        /// <param name="bucketName">The name of the S3 bucket to list objects from.</param>
        /// <param name="key">The prefix (folder path) to filter the listed objects. Only objects with this prefix will be returned.</param>
        /// <returns>A ListObjectsV2Request object configured for listing objects within a folder.</returns>
        private static ListObjectsV2Request CreateListRequest(string bucketName, string key)
        {
            return new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = key
            };
        }

        /// <summary>
        /// Asynchronously deletes all objects listed in a paginated response from Amazon S3.
        /// Iterates through pages of results from the ListObjectsV2 operation, 
        /// deleting each object using DeleteObject and handling continuation tokens for pagination.
        /// 
        /// Assumes DeleteObject is an asynchronous method that deletes an object from S3.
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="bucketName">The name of the S3 bucket containing the objects to delete.</param>
        /// <param name="listRequest">A ListObjectsV2Request object configured to list objects (assumed to be for deletion).</param>
        /// <returns>An awaitable Task representing the asynchronous deletion operation.</returns>
        private static async Task DeleteObjectsInList(AmazonS3Client s3Client, string bucketName, ListObjectsV2Request listRequest)
        {
            ListObjectsV2Response listResponse;
            do
            {
                listResponse = await s3Client.ListObjectsV2Async(listRequest);

                foreach (S3Object obj in listResponse.S3Objects)
                {
                    await DeleteObject(s3Client, bucketName, obj.Key);
                }

                listRequest.ContinuationToken = listResponse.NextContinuationToken;
            } while (listResponse.IsTruncated);
        }

        /// <summary>
        /// Asynchronously deletes a specific object from an Amazon S3 bucket.
        /// Creates a DeleteObjectRequest object and uses the S3 client to delete the object.
        /// 
        /// Assumes the S3 client has been configured with proper credentials and region.
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="bucketName">The name of the S3 bucket containing the object to delete.</param>
        /// <param name="key">The key (filename) of the object to delete in S3.</param>
        /// <returns>An awaitable Task representing the asynchronous deletion operation.</returns>
        private static async Task DeleteObject(AmazonS3Client s3Client, string bucketName, string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await s3Client.DeleteObjectAsync(deleteRequest);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            UIUpdater = null;
        }
    }
}
