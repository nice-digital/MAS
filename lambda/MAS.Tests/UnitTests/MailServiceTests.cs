using MAS.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;
using MAS.Services;
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MAS.Configuration;
using Moq;
using MailChimp.Net.Models;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests : TestBase
    {
        [Fact]
        public async Task SendToMailChimpAsync()
        {
            //Arrange
            var campaign = new Campaign();
            var ReturnedCampaign = Task.Run(() =>
            {
                return new Campaign()
                {
                    Id = "1234"
                };
            });

            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(campaign)).Returns(() => ReturnedCampaign);

            var _mailService = new MailService(mockMailChimpManager.Object);

            //Act
            await _mailService.CreateAndSendCampaignAsync("Test Subject", "Preview Text", "Body Text");

            //Assert
        }
    }
}
