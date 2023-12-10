﻿using Amazon.S3;
using Amazon.S3.Model;
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
        public static string UploadFiletoS3fromZip(AmazonS3Client s3Client, Byte[] fileByteArray,string fileName, string bucketName)
        {
            try
            {
                fileName = Path.ChangeExtension(fileName, null); // Remove the existing extension
                fileName = String.Format("{0}-{1:yyyy-MM-dd-HH-mmss}.zip", fileName, DateTime.Now);
                using (MemoryStream fileToUpload = new MemoryStream(fileByteArray))
                {
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        InputStream = fileToUpload,
                        ContentType = "application/zip"
                    };
                    request.Timeout = TimeSpan.FromSeconds(60);

                    PutObjectResponse response = s3Client.PutObject(request);

                    string s3Uri = $"s3://{bucketName}/{fileName}";

                    if(response.HttpStatusCode== HttpStatusCode.OK)
                    {
                        Console.WriteLine($"S3 URI of the uploaded file: {s3Uri}");
                        Console.WriteLine(response.HttpStatusCode);
                        return fileName;
                    }
                    else
                    {
                        return null;
                    }
                    // insert error trappings
                }
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
                        string s3Uri = $"s3://{bucketName}/{folderKey}/";
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
    }
}