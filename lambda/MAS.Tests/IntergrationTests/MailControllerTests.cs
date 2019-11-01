using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Moq;
using MAS.Tests.Infrastructure;
using MAS.Services;
using MAS.Models;
using MAS.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MAS.Tests.IntergrationTests
{
    public class MailControllerTests : TestBase
    {
        [Fact]
        public async Task Put()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetItemAsync(It.IsAny<string>())).ReturnsAsync(new Item() { Id = "123", Title = "My Test Drug" });

            var mockMailService = new Mock<IMailService>();
            mockMailService.Setup(x => x.CreateAndSendCampaignAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var mailController = new MailController(mockMailService.Object, mockContentService.Object);

            //Act
            var response = await mailController.PutAsync() as JsonResult;

            // Assert
            response.Value.ShouldBe("posted");
        }
    }
}
