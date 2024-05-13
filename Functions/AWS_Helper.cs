using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SageMaker;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// using Amazon.IdentityManagement.Model;
using Amazon;
using Amazon.ServiceQuotas;
using Amazon.ServiceQuotas.Model;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LSC_Trainer.Functions
{
    /// <summary>
    /// Provides helper methods for interacting with Amazon Web Services (AWS).
    /// </summary>
    public class AWS_Helper
    {
        /// <summary>
        /// Represents the total size of data uploaded.
        /// </summary>
        private static long totalUploaded = 0;

        /// <summary>
        /// Validates the provided access key ID by retrieving the username associated with it using the IAM client.
        /// Updates the UserConnectionInfo.UserName property with the retrieved username if the key is valid.
        /// </summary>
        /// <param name="iamClient">An AmazonIdentityManagementServiceClient instance used to interact with IAM services.</param>
        public static void CheckCredentials (AmazonIdentityManagementServiceClient iamClient)
        {
            var accessKeyLastUsedRequest = new GetAccessKeyLastUsedRequest
            {
                AccessKeyId = UserConnectionInfo.AccessKey
            };
            var response = iamClient.GetAccessKeyLastUsed(accessKeyLastUsedRequest);
            UserConnectionInfo.UserName = response.UserName;
        }

        /// <summary>
        /// Deletes all files in the provided dataset directory from an Amazon S3 bucket asynchronously.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client used to perform the deletion operation.</param>
        /// <param name="bucketName">The name of the Amazon S3 bucket containing the dataset objects.</param>
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
                    Prefix = $"users/{UserConnectionInfo.UserName}/training-jobs",
                });

                return response.S3Objects
                    .Select(o => new { Key = o.Key.Split('/')[3], Date = ExtractDateFromKey(o.Key) })
                    .OrderByDescending(o => o.Date) 
                    .Select(o => o.Key)
                    .Distinct()
                    .ToList();

               // return sortedKeys;
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
        /// Attempts to extract a date in the format "yyyy-MM-dd" from the provided key string using regular expressions.
        /// </summary>
        /// <param name="key">The string to search for a date format.</param>
        /// <returns>A DateTime object representing the extracted date if found, null otherwise.</returns>
        private static DateTime? ExtractDateFromKey(string key)
        {
            var regex = new Regex(@"\b\d{4}-\d{2}-\d{2}\b");
            var match = regex.Match(key);
            if (match.Success && DateTime.TryParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the URI and image tag of the first repository in Amazon Elastic Container Registry (ECR).
        /// </summary>
        /// <param name="accessKey">The access key used for authentication.</param>
        /// <param name="secretKey">The secret key used for authentication.</param>
        /// <param name="region">The AWS region to connect to.</param>
        /// <returns>
        /// The URI of the first repository in ECR if available; otherwise, returns null.
        /// </returns>
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
                        RepositoryName = firstRepo.RepositoryName
                    });

                    if (imageResponse.ImageDetails.Count > 0)
                    {
                        var latestImage = imageResponse.ImageDetails
                            .OrderByDescending(img => img.ImagePushedAt)
                            .First();

                        foreach (var tag in latestImage.ImageTags)
                        {
                            if (tag != "latest")
                            {
                                return (firstRepo.RepositoryUri, tag);
                            }
                        }
                    }
                }

                return (null, null);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of all Spot Training quotas from Amazon Service Quotas.
        /// Filters the quotas to include only supported instances related to Spot Training and with a value greater than or equal to 1.
        /// Orders the filtered quotas by name and returns them as a list of tuples (QuotaName, QuotaValue).
        /// </summary>
        /// <param name="serviceQuotasClient">An AmazonServiceQuotasClient instance used to interact with Service Quotas services.</param>
        /// <returns>A Task that resolves to a list of tuples (QuotaName, QuotaValue) representing the filtered Spot Training quotas, or an empty list if no quotas are found.</returns>
        public static async Task<List<(string QuotaName, double QuotaValue)>> GetAllSpotTrainingQuotas(AmazonServiceQuotasClient serviceQuotasClient)
        {
            var allInstances = new List<(string QuotaName, double QuotaValue)>();
            string nextToken = null;
            var desiredInstances = new List<string> { "ml.m5.large", "ml.m5.xlarge", "ml.g4dn.xlarge", "ml.g4dn.12xlarge", "ml.p3.16xlarge" };

            do
            {
                var listQuotasRequest = new ListServiceQuotasRequest()
                {
                    ServiceCode = "sagemaker",
                    MaxResults = 100,
                    NextToken = nextToken
                };

                var response = await serviceQuotasClient.ListServiceQuotasAsync(listQuotasRequest);

                foreach (var quota in response.Quotas)
                {
                    var instanceType = quota.QuotaName.Replace(" for spot training job usage", "");
                    if (quota.QuotaName.Contains("for spot training job usage") && quota.Value >= 1 && desiredInstances.Contains(instanceType))
                    {
                        allInstances.Add((instanceType, quota.Value));
                    }
                }

                nextToken = response.NextToken;
            } while (nextToken != null);

            allInstances = allInstances.OrderBy(instance => desiredInstances.IndexOf(instance.QuotaName)).ToList();

            return allInstances;
        }

        /// <summary>
        /// Asynchronously retrieves a list of available user-uploaded datasets from a specific Amazon S3 bucket.
        /// Filters the retrieved objects to include only files within the "users/{username}/custom-uploads" prefix.
        /// Returns a list of dataset names (filenames without path) if successful, null otherwise.
        /// </summary>
        /// <param name="s3Client">An AmazonS3Client instance used to interact with S3 services.</param>
        /// <param name="bucketName">The name of the S3 bucket to list objects from.</param>
        /// <returns>A Task that resolves to a list of dataset names (filenames) if successful, null on errors.</returns>
        public static async Task<List<string>> GetAvailableDatasetsList(AmazonS3Client s3Client, string bucketName)
        {
            try
            {
                List<string> customUploads = new List<string>();
                string continuationToken = null;

                do
                {
                    ListObjectsV2Response response = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
                    {
                        BucketName = bucketName,
                        Prefix = $"users/{UserConnectionInfo.UserName}/custom-uploads",
                        StartAfter = $"users/{UserConnectionInfo.UserName}/custom-uploads",
                        ContinuationToken = continuationToken
                    });

                    customUploads.AddRange(response.S3Objects
                        .Select(o => o.Key.Split('/')[3])
                        .Distinct());

                    continuationToken = response.NextContinuationToken;
                }
                while (continuationToken != null);

                return customUploads.Distinct().ToList();
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
