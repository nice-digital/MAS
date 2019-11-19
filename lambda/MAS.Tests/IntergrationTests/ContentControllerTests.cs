using System.Threading.Tasks;
using Xunit;
using MAS.Tests.Infrastructure;
using Shouldly;
using Amazon.S3;
using Amazon;
using MAS.Configuration;
using System.IO;

namespace MAS.Tests.IntergrationTests.Content
{
    public class ContentControllerTests : TestBase
    {
        [Fact]
        public async Task PutCMSItemSavesItemIntoS3()
        {
            //Arrange 
            AppSettings.CMSConfig = TestAppSettings.GetSingleItemFeed();

            AmazonS3Config config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.EUWest1,
                ServiceURL = AppSettings.AWSConfig.ServiceURL,
                ForcePathStyle = true
            };
            AmazonS3Client s3Client = new AmazonS3Client(AppSettings.AWSConfig.AccessKey, AppSettings.AWSConfig.SecretKey, config);

            //Act
            var response = await _client.PutAsync("/api/content/5daeb5af22565a82530d7373", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            using (var item = await s3Client.GetObjectAsync(AppSettings.AWSConfig.BucketName, "5daeb5af22565a82530d7373.txt"))
            {
                using (StreamReader reader = new StreamReader(item.ResponseStream))
                {
                    string contents = reader.ReadToEnd();
                    contents.ShouldBe("Wonder drug");
                }
            }
        }
            
    }
}
