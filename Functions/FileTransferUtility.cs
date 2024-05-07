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

namespace LSC_Trainer.Functions
{
    internal class FileTransferUtility : IFileTransferUtility
    {
        private static long totalUploaded = 0;
        private IUIUpdater UIUpdater { get; set; }
        public FileTransferUtility(IUIUpdater uIUpdater)
        {
            UIUpdater = uIUpdater;
        }
        public async Task UnzipAndUploadToS3(AmazonS3Client s3Client, string bucketName, string localZipFilePath, IProgress<int> progress)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                long totalSize = CalculateTotalSize(localZipFilePath);

                using (var fileStream = File.OpenRead(localZipFilePath))
                {
                    using (var zipStream = new ZipInputStream(fileStream))
                    {
                        await ProcessZipEntries(s3Client, zipStream, bucketName, progress, totalSize);
                    }
                }

                Console.WriteLine("Successfully uploaded all files from the folder to S3.");
                LogUploadTime(startTime);
            }
            catch (AmazonS3Exception e)
            {
                LogError("Error uploading file to S3 here: ", e);
            }
            catch (Exception e)
            {
                LogError("Error uploading file to S3: ", e);
            }
        }

        public async Task<string> UploadFileToS3(AmazonS3Client s3Client, string filePath, string fileName, string bucketName, IProgress<int> progress, long totalSize)
        {
            try
            {
                DateTime startTime = DateTime.Now;

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityUploadRequest uploadRequest = CreateUploadRequest(filePath, fileName, bucketName);
                    ConfigureProgressTracking(uploadRequest, progress, totalSize, UIUpdater);

                    await transferUtility.UploadAsync(uploadRequest);
                }

                LogUploadTime(startTime);
                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                LogError("Error uploading file to S3: ", e);
                return null;
            }
            catch (Exception e)
            {
                LogError("Error uploading file to S3: ", e);
                return null;
            }
        }

        private readonly SemaphoreSlim trackUploadLock = new SemaphoreSlim(1, 10);
        public async Task<string> UploadFileToS3(AmazonS3Client s3Client, MemoryStream fileStream, string fileName, string bucketName, IProgress<int> progress, long totalSize)
        {
            try
            {
                await trackUploadLock.WaitAsync();
                DateTime startTime = DateTime.Now;
                try
                {
                    
                    using (TransferUtility transferUtility = new TransferUtility(s3Client))
                    {
                        TransferUtilityUploadRequest uploadRequest = CreateUploadRequest(fileStream, fileName, bucketName);
                        ConfigureProgressTracking(uploadRequest, progress, totalSize, UIUpdater);

                        await transferUtility.UploadAsync(uploadRequest);
                    }
                }
                finally
                {
                    trackUploadLock.Release();
                }
                

                LogUploadTime(startTime);
                return fileName;
            }
            catch (AmazonS3Exception e)
            {
                LogError("Error uploading file to S3: ", e);
                return null;
            }
            catch (Exception e)
            {
                LogError("Error uploading file to S3: ", e);
                return null;
            }
        }

        public async Task UploadFolderToS3(AmazonS3Client s3Client, string folderPath, string folderName, string bucketName, IProgress<int> progress)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                long totalSize = CalculateTotalSizeFolder(folderPath);
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

                var tasks = files.Select(file =>
                {
                    var key = GenerateKey(folderPath, file, folderName);
                    return UploadFileToS3(s3Client, file, key, bucketName, progress, totalSize);
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

        public async Task<string> DownloadObjects(AmazonS3Client s3Client, string bucketName, string objectKey, string localFilePath)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                string filePath = PrepareLocalFile(localFilePath, objectKey);

                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityDownloadRequest downloadRequest = CreateDownloadRequest(bucketName, objectKey, filePath);

                    ConfigureProgressTracking(downloadRequest, UIUpdater);

                    await transferUtility.DownloadAsync(downloadRequest);
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

        private static void ConfigureProgressTracking(TransferUtilityUploadRequest uploadRequest, IProgress<int> progress, long totalSize, IUIUpdater UIUpdater)
        {
            long currentFileUploaded = 0;

            uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
            {
                currentFileUploaded = args.TransferredBytes;
                if (args.PercentDone == 100)
                {
                    totalUploaded += currentFileUploaded;
                    int overallPercentage = (int)(totalUploaded * 100 / totalSize);
                    progress.Report(overallPercentage);
                    UIUpdater.UpdateTrainingStatus($"Uploading Files to S3", $"Uploading {totalUploaded}/{totalSize} - {overallPercentage}%");
                }
            });
        }

        private static void ConfigureProgressTracking(TransferUtilityDownloadRequest downloadRequest, IUIUpdater UIUpdater)
        {

            downloadRequest.WriteObjectProgressEvent += (sender, args) =>
            {
                int percentage = (int)(args.TransferredBytes * 100 / args.TotalBytes);
                UIUpdater.UpdateTrainingStatus($"Downloading Files from S3", $"Downloading {args.TransferredBytes}/{args.TotalBytes} - {percentage}%");
            };
        }

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

        private static void LogError(string message, Exception e)
        {
            Console.WriteLine($"{message} {e.Message}");
        }
        private static string GenerateKey(string folderPath, string file, string folderName)
        {
            var relativePath = PathHelper.GetRelativePath(folderPath, file);
            var key = relativePath.Replace(Path.DirectorySeparatorChar, '/');
            return folderName + "/" + key;
        }

        private async Task ProcessZipEntries(AmazonS3Client s3Client, ZipInputStream zipStream, string bucketName, IProgress<int> progress, long totalSize)
        {
            ZipEntry entry;
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                if (!entry.IsDirectory)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await DecompressEntryAsync(zipStream, memoryStream);

                        string fileName = "custom-uploads/" + entry.Name;
                        await UploadFileToS3(s3Client, memoryStream, fileName, bucketName, progress, totalSize);
                    }
                }
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
            byte[] buffer = new byte[8192];
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
                    return "application/octet-stream";
            }
        }

        private static string PrepareLocalFile(string localFilePath, string objectKey)
        {
            string directoryPath = Path.GetDirectoryName(localFilePath);
            Directory.CreateDirectory(directoryPath);
            return Path.Combine(localFilePath, objectKey.Split('/').Last());
        }

        private static TransferUtilityDownloadRequest CreateDownloadRequest(string bucketName, string objectKey, string filePath)
        {
            return new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                FilePath = filePath
            };
        }

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

        private static ListObjectsV2Request CreateListRequest(string bucketName, string key)
        {
            return new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = key
            };
        }

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

        private static async Task DeleteObject(AmazonS3Client s3Client, string bucketName, string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}
