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
                Source = "https://www.google.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/api/content/", content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
