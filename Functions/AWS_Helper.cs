using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LSC_Trainer.Functions
{
    class AWS_Helper
    {
        public static string UploadFileToS3(AmazonS3Client s3Client, string filePath, string fileName, string bucketName)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                string extension = Path.GetExtension(fileName);
                fileName = Path.ChangeExtension(fileName, null); // Remove the existing extension
                /*string content = "";

                if(extension == ".rar")
                {
                    fileName = String.Format("{0}-{1:yyyy-MM-dd-HH-mmss}.rar", fileName, DateTime.Now);
                    content = "rar";
                }
                if(extension == ".zip")
                {
                    fileName = String.Format("{0}-{1:yyyy-MM-dd-HH-mmss}.zip", fileName, DateTime.Now);
                    content = "zip";
                }
                */
                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        FilePath = filePath,
                        ContentType = GetContentType(fileName)
                    };

                    uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
                    {
                        Console.WriteLine($"Progress: {args.PercentDone}%");
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
                Console.WriteLine($"Upload completed. Total Time Taken: {formattedTotalTime}");
                Console.WriteLine($"S3 URI of the uploaded file: {s3Uri}");
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

        public static async Task UploadFolderToS3(AmazonS3Client s3Client, string folderPath,string folderName,string bucketName)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var relativePath = PathHelper.GetRelativePath(folderPath, file);
                    var key = relativePath.Replace(Path.DirectorySeparatorChar, '/');
                    key = folderName + "/" + key;
                    UploadFileToS3(s3Client, file, key, bucketName);
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

        public static async Task UnzipAndUploadToS3(AmazonS3Client s3Client, string bucketName, string localZipFilePath)
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

                                var uploadRequest = new TransferUtilityUploadRequest
                                {
                                    BucketName = bucketName,
                                    Key = entry.Name,
                                    InputStream = memoryStream,
                                    ContentType = GetContentType(entry.Name)
                                };

                                // Upload the memory stream to S3
                                uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
                                {
                                    Console.WriteLine($"Progress: {args.PercentDone}%, {args.TransferredBytes} bytes /{args.TotalBytes} bytes");
                                });
                                await transferUtility.UploadAsync(uploadRequest);
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
                Console.WriteLine("Error uploading file to S3: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error uploading file to S3: " + e.Message);
            }
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


        public static async Task DownloadFile(AmazonS3Client s3Client, string bucketName, string objectKey, string localFilePath)
        {
            try
            {
                GetObjectResponse response = await s3Client.GetObjectAsync(bucketName, objectKey);

                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(localFilePath);
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine(directoryPath);
                string filePath = Path.Combine(localFilePath, objectKey.Split('/').Last());
                using (var fileStream = File.OpenWrite(filePath))
                {
                    await response.ResponseStream.CopyToAsync(fileStream);
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine($"File has been saved to {localFilePath}");
                    }
                }
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error downloading file: " + e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Error downloading file: " + e.Message);
                Console.WriteLine($"UnauthorizedAccessException at: {e.StackTrace}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error downloading file: " + e.Message);
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
