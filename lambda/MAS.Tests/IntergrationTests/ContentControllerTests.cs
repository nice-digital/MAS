using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using MAS.Tests.Infrastructure;
using System.Net.Http;
using System.Text;
using System.Net;
using Shouldly;

namespace MAS.Tests.IntergrationTests
{
    public class ContentControllerTests : TestBase
    {
        [Fact]
        public async Task Put()
        {
            //Arrange
            var content = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/api/content/1234", content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
