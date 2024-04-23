using Xunit;
using LSC_Trainer;

namespace LSC_Trainer.Tests
{
    public class UserConnectionInfoTests
    {

        public UserConnectionInfoTests()
        {

            UserConnectionInfo.AccountId = "TestAccountId";
            UserConnectionInfo.AccessKey = "TestAccessKey";
            UserConnectionInfo.SecretKey = "TestSecretKey";
            UserConnectionInfo.Region = "TestRegion";
            UserConnectionInfo.RoleArn = "TestRoleArn";
            UserConnectionInfo.EcrUri = "TestEcrUri";
            UserConnectionInfo.SetBucketAndURIs();
        }

        [Fact]
        public void TestInstance()
        {
            Assert.NotNull(UserConnectionInfo.Instance);
        }

        // hard to test because of static properties
        //[Fact]
        //public void TestSetBucketAndURIs()
        //{
        //    string expectedEcrUri = "TestEcrUri";
        //    string expectedSagemakerBucket = "sagemaker-TestRegion-TestAccountId";
        //    string expectedDefaultDatasetURI = $"s3://{expectedSagemakerBucket}/default-datasets/MMX059XA_COVERED5B/";
        //    string expectedCustomUploadsURI = $"s3://{expectedSagemakerBucket}/custom-uploads/";
        //    string expectedDestinationURI = $"s3://{expectedSagemakerBucket}/training-jobs/";

        //    Assert.Equal(expectedEcrUri, UserConnectionInfo.EcrUri);
        //    Assert.Equal(expectedSagemakerBucket, UserConnectionInfo.SagemakerBucket);
        //    Assert.Equal(expectedDefaultDatasetURI, UserConnectionInfo.DefaultDatasetURI);
        //    Assert.Equal(expectedCustomUploadsURI, UserConnectionInfo.CustomUploadsURI);
        //    Assert.Equal(expectedDestinationURI, UserConnectionInfo.DestinationURI);
        //}

        [Fact]
        public void TestReset()
        {
            UserConnectionInfo.Instance.Reset();
            Assert.Null(UserConnectionInfo.AccountId);
            Assert.Null(UserConnectionInfo.AccessKey);
            Assert.Null(UserConnectionInfo.SecretKey);
            Assert.Null(UserConnectionInfo.Region);
            Assert.Null(UserConnectionInfo.RoleArn);
            Assert.Null(UserConnectionInfo.EcrUri);
        }

        
    }
}