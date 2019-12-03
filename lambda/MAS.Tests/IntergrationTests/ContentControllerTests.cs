using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using MAS.Tests.Infrastructure;
using System.Net.Http;
using System.Text;
using System.Net;
using Shouldly;
using MAS.Configuration;
using MAS.Models;

namespace MAS.Tests.IntergrationTests
{
    public class ContentControllerTests : TestBase
    {
        public ContentControllerTests()
        {
            AppSettings.AWSConfig = TestAppSettings.GetAWSConfig();
        }

        [Fact]
        public async Task Put()
        {
            //Arrange
            Item item = new Item()
            {
                Id = "1234",
                Title = "Some title",
                ShortSummary = "Short summary",
                Source = "https://www.google.com",
                EvidenceType = "Evidence Type",
                UKMiComment = "UKMI Comment",
                ResourceLinks = "<p><a title=\"Link 1\" href=\"items/5de65fe432281d43fbfcd15a\">Link 1</a></p>\r\n<p><a title=\"sadada\" href=\"items/5de65fe432281d43fbfcd15a\">Link 2</a></p>"
            };

            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/api/content/", content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
