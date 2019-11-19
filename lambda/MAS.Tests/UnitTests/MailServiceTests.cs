using MAS.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;
using MAS.Services;
using MailChimp.Net.Interfaces;
using Moq;
using MailChimp.Net.Models;
using MailChimp.Net.Core;
using Shouldly;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests : TestBase
    {
        [Fact]
        public void CreateCampaignAndSendToMailChimp()
        {
            //Arrange
            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            mockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            mockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>()));

            var mailService = new MailService(mockMailChimpManager.Object);

            //Act
            var response = mailService.CreateAndSendCampaignAsync("Test Subject", "Preview Text", "Body Text");
            
            //Assert
            response.Exception.ShouldBe(null);
            response.Result.ShouldBe("1234");
            response.Status.ShouldBe(TaskStatus.RanToCompletion);
        }
    }
}
