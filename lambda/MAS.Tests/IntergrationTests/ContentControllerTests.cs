using System.Threading.Tasks;
using Xunit;
using MAS.Tests.Infrastructure;
using Shouldly;
using Amazon.S3;

namespace MAS.Tests.IntergrationTests
{
    public class ContentControllerTests : TestBase
    {
        [Fact]
        public async Task Put()
        {
            //Act
            var response = await _client.PutAsync("/api/content/5dc9402a79798e2ab20c6ab6", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }
    }
}
