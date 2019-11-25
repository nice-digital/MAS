using System.Threading.Tasks;
using Xunit;
using MAS.Tests.Infrastructure;
using Shouldly;
using Amazon.S3;
using Amazon;
using MAS.Configuration;
using System.IO;
using MAS.Models;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

namespace MAS.Tests.IntergrationTests.Content
{
    public class ContentControllerTests : TestBase
    {
        [Fact]
        public async Task PutCMSItemSavesItemIntoS3()
        {
            //Arrange 
            AmazonS3Config config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.EUWest1,
                ServiceURL = AppSettings.AWSConfig.ServiceURL,
                ForcePathStyle = true
            };
            AmazonS3Client s3Client = new AmazonS3Client(AppSettings.AWSConfig.AccessKey, AppSettings.AWSConfig.SecretKey, config);
            
            Item item = new Item()
            {
                Id = "1234",
                Title = "Some title",
                ShortSummary = "Wonder drug",
                Source = "https://www.google.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/api/content/", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PutObjectResponse>(responseJson);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            responseObject.ETag.ShouldNotBeNull();
           
            using (var bucketItem = await s3Client.GetObjectAsync(AppSettings.AWSConfig.BucketName, "5daeb5af22565a82530d7373.txt"))
            {
                using (StreamReader reader = new StreamReader(bucketItem.ResponseStream))
                {
                    string contents = reader.ReadToEnd();
                    contents.ShouldBe("Wonder drug");
                }
            }
        }
            
    }
}
