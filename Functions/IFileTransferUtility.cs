﻿using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC_Trainer.Functions
{
    public interface IFileTransferUtility
    {
        string UploadFileToS3(AmazonS3Client s3Client, string filePath, string fileName, string bucketName, IProgress<int> progress, long totalSize);
        string UploadFileToS3(AmazonS3Client s3Client, MemoryStream fileStream, string fileName, string bucketName, IProgress<int> progress, long totalSize);
        Task UploadFolderToS3(AmazonS3Client s3Client, string folderPath, string folderName, string bucketName, IProgress<int> progress);
        Task UnzipAndUploadToS3(AmazonS3Client s3Client, string bucketName, string localZipFilePath, IProgress<int> progress);
        Task<string> DownloadObjects(AmazonS3Client s3Client, string bucketName, string objectKey, string localFilePath);
        Task DeleteDataSet(AmazonS3Client s3Client, string bucketName, string key);
    }
}