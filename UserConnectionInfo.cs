namespace LSC_Trainer
{
    public class UserConnectionInfo
    {
        private static UserConnectionInfo _instance;

        public static string AccountId { get; set; }
        public static string AccessKey { get; set; }
        public static string SecretKey { get; set; }
        public static string Region { get; set; }
        public static string RoleArn { get; set; }
        public static string EcrUri { get; set; }
        public static string SagemakerBucket { get; set; }
        public static string DefaultDatasetURI { get; set; }
        public static string CustomUploadsURI { get; set; }
        public static string DestinationURI { get; set; }

        private UserConnectionInfo() { }

        public static UserConnectionInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserConnectionInfo();
                }
                return _instance;
            }
        }

        public void Reset()
        {
            AccountId = null;
            AccessKey = null;
            SecretKey = null;
            Region = null;
            RoleArn = null;
            EcrUri = null;
        }

        public static void SetBucketAndURIs()
        {
            EcrUri = Properties.Settings.Default.CITU_DevTeam_ECR_URI;
            SagemakerBucket = $"sagemaker-{Region}-{AccountId}";
            DefaultDatasetURI = $"s3://{SagemakerBucket}/default-datasets/MMX059XA_COVERED5B/";
            CustomUploadsURI = $"s3://{SagemakerBucket}/custom-uploads/";
            DestinationURI = $"s3://{SagemakerBucket}/training-jobs/";
        }
    }
}
