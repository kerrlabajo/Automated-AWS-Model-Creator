using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SageMaker;
using Amazon.ECR;
using Amazon.ECR.Model;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// using Amazon.IdentityManagement.Model;
using Amazon;

namespace LSC_Trainer.Functions
{
    /// <summary>
    /// Provides helper methods for interacting with Amazon Web Services (AWS).
    /// </summary>
    class AWS_Helper
    {
        /// <summary>
        /// Represents the total size of data uploaded.
        /// </summary>
        private static long totalUploaded = 0;

        /// <summary>
        /// Tests the connection to the Amazon SageMaker client by listing training jobs.
        /// </summary>
        /// <param name="client">The Amazon SageMaker client to be tested.</param>
        public static void TestSageMakerClient (AmazonSageMakerClient client)
        {
            client.ListTrainingJobs(new Amazon.SageMaker.Model.ListTrainingJobsRequest());
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
        public static string UploadFileToS3(AmazonS3Client s3Client, string filePath, string fileName, string bucketName, IProgress<int> progress, long totalSize)
        {
            try
            {
                DateTime startTime = DateTime.Now;

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        FilePath = filePath,
                        ContentType = GetContentType(fileName)
                    };

                    long currentFileUploaded = 0;

                    uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
                    {
                        currentFileUploaded = args.TransferredBytes;
                        if (args.PercentDone == 100)
                        {
                            totalUploaded += currentFileUploaded;
                            int overallPercentage = (int)(totalUploaded * 100 / totalSize);
                            progress.Report(overallPercentage);
                        }
                    });

                    transferUtility.Upload(uploadRequest);
                }

                string s3Uri = $"s3://{bucketName}/{fileName}";
                TimeSpan totalTime = DateTime.Now - startTime;
                string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                              (int)totalTime.TotalHours,
                                              totalTime.Minutes,
                                              totalTime.Seconds,
                                              (int)(totalTime.Milliseconds / 100));

                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
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
        public static string UploadFileToS3(AmazonS3Client s3Client, MemoryStream fileStream, string fileName, string bucketName, IProgress<int> progress, long totalSize)
        {
            try
            {
                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        InputStream = fileStream,
                        ContentType = GetContentType(fileName)
                    };

                    long currentFileUploaded = 0;

                    uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
                    {
                        currentFileUploaded = args.TransferredBytes;
                        if (args.PercentDone == 100)
                        {
                            totalUploaded += currentFileUploaded;
                            int overallPercentage = (int)(totalUploaded * 100 / totalSize);
                            progress.Report(overallPercentage);
                        }
                    });

                    transferUtility.Upload(uploadRequest);
                }

                    
                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Uploads a folder and its contents to Amazon S3 asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client instance.</param>
        /// <param name="folderPath">The local path of the folder to upload.</param>
        /// <param name="folderName">The name of the folder in S3 where the files will be stored.</param>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="progress">An instance of IProgress to report the progress of the upload.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public static async Task UploadFolderToS3(AmazonS3Client s3Client, string folderPath,string folderName,string bucketName, IProgress<int> progress)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                long totalSize = CalculateTotalSizeFolder(folderPath);
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var relativePath = PathHelper.GetRelativePath(folderPath, file);
                    var key = relativePath.Replace(Path.DirectorySeparatorChar, '/');
                    key = folderName + "/" + key;
                    UploadFileToS3(s3Client, file, key, bucketName, progress, totalSize);
                }

                Console.WriteLine("Successfully uploaded all files from the zip to S3.");
                TimeSpan totalTime = DateTime.Now - startTime;
                string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                              (int)totalTime.TotalHours,
                                              totalTime.Minutes,
                                              totalTime.Seconds,
                                              (int)(totalTime.Milliseconds / 100));
                Console.WriteLine($"Upload completed. Total Time Taken: {formattedTotalTime}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error uploading folder to S3: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error uploading folder to S3: " + e.Message);
            }
        }

        /// <summary>
        /// Extracts the contents of a ZIP file into Memory Stream and uploads them to Amazon S3 asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client instance.</param>
        /// <param name="bucketName">The name of the S3 bucket where the files will be uploaded.</param>
        /// <param name="localZipFilePath">The local path of the ZIP file to extract and upload.</param>
        /// <param name="progress">An instance of IProgress to report the progress of the upload.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public static async Task UnzipAndUploadToS3(AmazonS3Client s3Client, string bucketName, string localZipFilePath, IProgress<int> progress)
        {
            try
            {
                DateTime startTime = DateTime.Now;

                using (var fileStream = File.OpenRead(localZipFilePath))
                {
                    using (var zipStream = new ZipInputStream(fileStream))
                    {
                        var transferUtility = new TransferUtility(s3Client);

                        ZipEntry entry;
                        while ((entry = zipStream.GetNextEntry()) != null)
                        {
                            if (entry.IsDirectory)
                            {
                                continue;
                            }

                            using (var memoryStream = new MemoryStream())
                            {
                                // Decompress data into the memory stream
                                await DecompressEntryAsync(zipStream, memoryStream);

                                long totalSize = CalculateTotalSize(localZipFilePath);
                                string fileName = "custom-uploads/" + entry.Name;
                                UploadFileToS3(s3Client, memoryStream, fileName, bucketName, progress, totalSize);
                            }
                        }

                        Console.WriteLine("Successfully uploaded all files from the zip to S3.");
                        TimeSpan totalTime = DateTime.Now - startTime;
                        string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            (int)totalTime.TotalHours,
                            totalTime.Minutes,
                            totalTime.Seconds,
                            (int)(totalTime.Milliseconds / 100));
                        Console.WriteLine($"Upload completed. Total Time Taken: {formattedTotalTime}");
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error uploading file to S3 here: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
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
            byte[] buffer = new byte[8192]; // Adjust buffer size as needed
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
                    return "application/octet-stream"; // Default MIME type for unknown file types
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
        public static async Task<string> DownloadObjects(AmazonS3Client s3Client, string bucketName, string objectKey, string localFilePath)
        {
            string response = "";
            try
            {
                DateTime startTime = DateTime.Now;
                string directoryPath = Path.GetDirectoryName(localFilePath);
                Directory.CreateDirectory(directoryPath);
                string filePath = Path.Combine(localFilePath, objectKey.Split('/').Last());

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityDownloadRequest downloadRequest = new TransferUtilityDownloadRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey,
                        FilePath = filePath
                    };

                    await transferUtility.DownloadAsync(downloadRequest);

                    response += $"{Environment.NewLine} File has been saved to {filePath} {Environment.NewLine}";
                    TimeSpan totalTime = DateTime.Now - startTime;
                    string formattedTotalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        (int)totalTime.TotalHours,
                        totalTime.Minutes,
                        totalTime.Seconds,
                        (int)(totalTime.Milliseconds / 100));
                    response += $"Total Time Taken: {formattedTotalTime}";
                }
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
            return response;
        }

        public static async Task DeleteDataSet(AmazonS3Client s3Client, string bucketName, string key)
        {
            ListObjectsV2Request listRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = key
            };

            ListObjectsV2Response listResponse;
            do
            {
                // Get the list of objects
                listResponse = await s3Client.ListObjectsV2Async(listRequest);

                // Delete each object
                foreach (S3Object obj in listResponse.S3Objects)
                {
                    var deleteRequest = new DeleteObjectRequest
                    {
                        BucketName = bucketName,
                        Key = obj.Key
                    };

                    await s3Client.DeleteObjectAsync(deleteRequest);
                }

                // Set the marker property
                listRequest.ContinuationToken = listResponse.NextContinuationToken;
            } while (listResponse.IsTruncated);

            MessageBox.Show($"Deleted dataset: {key}");
        }
        /// <summary>
        /// Deletes all files in the provided dataset directory from an Amazon S3 bucket asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client used to perform the deletion operation.</param>
        /// <param name="bucketName">The name of the Amazon S3 bucket containing the dataset objects.</param>
        /// <param name="key">The key of the dataset directory to delete from the Amazon S3 bucket.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="AmazonS3Exception">Thrown when an error occurs during the Amazon S3 operation.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public static async Task<List<string>> GetTrainingJobOutputList(AmazonS3Client s3Client, string bucketName)
        {
            try
            {
                var response = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = "training-jobs"
                });

                return response.S3Objects
                    .Select(o => o.Key.Split('/')[1])
                    .Distinct()
                    .ToList();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error retrieving list from S3: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the URI of the first repository in Amazon Elastic Container Registry (ECR).
        /// </summary>
        /// <param name="accessKey">The access key used for authentication.</param>
        /// <param name="secretKey">The secret key used for authentication.</param>
        /// <param name="region">The AWS region to connect to.</param>
        /// <returns>
        /// The URI of the first repository in ECR if available; otherwise, returns null.
        /// </returns>
        public static string GetFirstRepositoryUri(string accessKey, string secretKey, RegionEndpoint region)
        {
            using (var ecrClient = new AmazonECRClient(accessKey, secretKey, region))
            {
                var response = ecrClient.DescribeRepositories(new DescribeRepositoriesRequest());

                if (response.Repositories.Count > 0)
                {
                    return response.Repositories[0].RepositoryUri;
                }
                else
                {
                    return null;
                }
            }
        }

    }

    /// <summary>
    /// Provides helper methods for working with file paths.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the relative path of a target path from a base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <returns>The relative path of the target path from the base path.</returns>
        public static string GetRelativePath(string basePath, string targetPath)
        {
            var baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);
            var targetUri = new Uri(targetPath);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(targetUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
