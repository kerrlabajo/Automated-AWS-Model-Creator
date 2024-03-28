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
    class AWS_Helper
    {

        private static long totalUploaded = 0;

        public static void TestSageMakerClient (AmazonSageMakerClient client)
        {
            client.ListTrainingJobs(new Amazon.SageMaker.Model.ListTrainingJobsRequest());
        }

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

        public static long CalculateTotalSizeFolder(string directoryPath)
        {
            Console.WriteLine($"Filename - {directoryPath}");
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            return dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

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


        private static async Task DecompressEntryAsync(ZipInputStream zipStream, MemoryStream memoryStream)
        {
            byte[] buffer = new byte[8192]; // Adjust buffer size as needed
            int bytesRead;
            while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, bytesRead);
            }
        }

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

        public static (string, string) GetFirstRepositoryUriAndImageTag(string accessKey, string secretKey, RegionEndpoint region)
        {
            using (var ecrClient = new AmazonECRClient(accessKey, secretKey, region))
            {
                var response = ecrClient.DescribeRepositories(new DescribeRepositoriesRequest());

                if (response.Repositories.Count > 0)
                {
                    var firstRepo = response.Repositories[0];
                    var imageResponse = ecrClient.DescribeImages(new DescribeImagesRequest
                    {
                        RepositoryName = firstRepo.RepositoryName,
                        ImageIds = new List<ImageIdentifier> { new ImageIdentifier { ImageTag = "latest" } }
                    });

                    if (imageResponse.ImageDetails.Count > 0)
                    {
                        return (firstRepo.RepositoryUri, imageResponse.ImageDetails[0].ImageTags[0]);
                    }
                }

                return (null, null);
            }
        }

    }


    public static class PathHelper
    {
        public static string GetRelativePath(string basePath, string targetPath)
        {
            var baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);
            var targetUri = new Uri(targetPath);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(targetUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
