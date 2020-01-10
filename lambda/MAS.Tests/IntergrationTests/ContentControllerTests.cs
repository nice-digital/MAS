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
                Source = new Source()
                {
                    Id = "789",
                    Title = "The Journal of Medicine"
                },
                EvidenceType = new EvidenceType
                {
                    Key = "mas_evidence_types:Safety%20alerts",
                    Title = "Safety alerts"
                },
                UKMiComment = "UKMI Comment",
                ResourceLinks = "<p><a title=\"Link 1\" href=\"items/5de65fe432281d43fbfcd15a\">Link 1</a></p>\r\n<p><a title=\"sadada\" href=\"items/5de65fe432281d43fbfcd15a\">Link 2</a></p>"
            };

            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/api/content/", content);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
           
            using (var bucketItem = await s3Client.GetObjectAsync(AppSettings.AWSConfig.BucketName, "1234.txt"))
            {
                using (StreamReader reader = new StreamReader(bucketItem.ResponseStream))
                {
                    string contents = reader.ReadToEnd();
                    contents.ShouldMatchApproved();
                }
            }
        }
            
    }
}
