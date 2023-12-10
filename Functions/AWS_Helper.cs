using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
                string content = "";

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
                
                using (TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        FilePath = filePath,
                        ContentType = "application/" + content
                    };

                    uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>((sender, args) =>
                    {
                        Console.WriteLine($"Progress: {args.PercentDone}%");
                    });

                    transferUtility.Upload(uploadRequest);
                }

                string s3Uri = $"s3://{bucketName}/{fileName}";
                TimeSpan totalTime = DateTime.Now - startTime;
                Console.WriteLine($"Upload completed. Total Time Taken: {totalTime}");
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

        public static async Task UnzipAndUploadFiles(AmazonS3Client s3Client, string bucketName, string zipKey)
        {
            try
            {
                // Download the Zip file
                GetObjectResponse response = await s3Client.GetObjectAsync(bucketName, zipKey);

                // Create a memory stream to hold the downloaded data
                using (var memoryStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // get file name for folder
                    string folderKey = Path.ChangeExtension(zipKey, null);
                    folderKey = folderKey + "/";
                    // Open the Zip archive
                    using (var zipArchive = new ZipArchive(memoryStream))
                    {
                        // Iterate through each file in the archive
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            using (var entryStream = entry.Open())
                            {
                                // Upload the individual file to S3
                                PutObjectResponse response2 = await s3Client.PutObjectAsync(new PutObjectRequest
                                {
                                    BucketName = bucketName,
                                    Key = folderKey + entry.FullName,
                                    InputStream = entryStream
                                });

                                if (response.HttpStatusCode == HttpStatusCode.OK)
                                {
                                    Console.WriteLine($"Successfully uploaded: {entry.FullName}");
                                }
                                else
                                {
                                    Console.WriteLine($"Failed to upload: {entry.FullName}");
                                }
                            }
                        }
                        string s3Uri = $"s3://{bucketName}/{folderKey}";
                        Console.WriteLine($"Successfully uploaded file to : {s3Uri}");
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
}
