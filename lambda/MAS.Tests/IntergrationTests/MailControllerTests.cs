using System.Threading.Tasks;
using Xunit;
using Shouldly;
using MAS.Tests.Infrastructure;

namespace MAS.Tests.IntergrationTests
{
    public class MailControllerTests : TestBase
    {
        [Fact]
        public async Task CreateAndSendCampaign()
        {
            //Act
            var response = await _client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }
    }
}
