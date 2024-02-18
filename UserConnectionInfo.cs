namespace LSC_Trainer
{
    class UserConnectionInfo
    {
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
    }
}
