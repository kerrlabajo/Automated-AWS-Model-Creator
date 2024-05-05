using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SageMaker;
using Amazon.ECR;
using Amazon.ECR.Model;
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
    public class AWS_Helper
    {

        private static long totalUploaded = 0;

        public static void TestSageMakerClient (AmazonSageMakerClient client)
        {
            client.ListTrainingJobs(new Amazon.SageMaker.Model.ListTrainingJobsRequest());
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
                    .Select(o => new { Key = o.Key.Split('/')[1], Date = ExtractDateFromKey(o.Key) })
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

        public static async Task<List<(string QuotaName, double QuotaValue)>> GetAllSpotTrainingQuotas(AmazonServiceQuotasClient serviceQuotasClient)
        {
            var allInstances = new List<(string QuotaName, double QuotaValue)>();
            string nextToken = null;

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
                    if (quota.QuotaName.Contains("for spot training job usage") && quota.Value >= 1)
                    {
                        allInstances.Add((quota.QuotaName.Replace(" for spot training job usage", ""), quota.Value));
                    }
                }

                nextToken = response.NextToken;
            } while (nextToken != null);

            allInstances = allInstances.OrderBy(instance => instance.QuotaName).ToList();

            return allInstances;
        }

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
                        Prefix = "custom-uploads",
                        StartAfter = "custom-uploads",
                        ContinuationToken = continuationToken
                    });

                    customUploads.AddRange(response.S3Objects
                        .Select(o => o.Key.Split('/')[1])
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
