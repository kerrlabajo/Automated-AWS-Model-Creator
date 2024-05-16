using Xunit;
using AutomatedAWSModelCreator;

namespace AutomatedAWSModelCreator.Tests
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

        [Fact]
        public void TestCredentialsSaved()
        {
            // Arrange
            var expectedAccountId = "testAccountId";
            var expectedAccessKey = "testAccessKey";
            var expectedSecretKey = "testSecretKey";
            var expectedRegion = "testRegion";
            var expectedRoleArn = "testRoleArn";

            // Act
            UserConnectionInfo.AccountId = expectedAccountId;
            UserConnectionInfo.AccessKey = expectedAccessKey;
            UserConnectionInfo.SecretKey = expectedSecretKey;
            UserConnectionInfo.Region = expectedRegion;
            UserConnectionInfo.RoleArn = expectedRoleArn;
            // Assert
            Assert.Equal(expectedAccountId, UserConnectionInfo.AccountId);
            Assert.Equal(expectedAccessKey, UserConnectionInfo.AccessKey);
            Assert.Equal(expectedSecretKey, UserConnectionInfo.SecretKey);
            Assert.Equal(expectedRegion, UserConnectionInfo.Region);
            Assert.Equal(expectedRoleArn, UserConnectionInfo.RoleArn);
        }

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