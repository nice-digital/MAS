using MAS.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;
using MAS.Services;
using MailChimp.Net.Interfaces;
using Moq;
using MailChimp.Net.Models;
using MailChimp.Net.Core;
using Shouldly;
using Microsoft.Extensions.Logging;
using MAS.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests
    {
        [Fact]
        public void CreateCampaignAndSendToMailChimp()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            mockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            mockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>()));
            mockMailChimpManager.Setup(x => x.Interests.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), null))
                .ReturnsAsync(new List<Interest>() { new Interest { Id = "987" } });

            var mailService = new MailService(mockMailChimpManager.Object, mockLogger.Object, Mock.Of<MailChimpConfig>(), Mock.Of<MailConfig>());

            //Act
            var response = mailService.CreateAndSendDailyAsync("Test Subject", "Preview Text", "Body Text", new List<string>());
            
            //Assert
            response.Exception.ShouldBe(null);
            response.Result.ShouldBe("1234");
            response.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        [Fact]
        public void ErrorInSendingCampaignShouldThrowError()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            mockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            mockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>())).Throws(new Exception());

            var mailService = new MailService(mockMailChimpManager.Object, mockLogger.Object, Mock.Of<MailChimpConfig>(), Mock.Of<MailConfig>());

            //Act + Assert
            Should.Throw<Exception>(() => mailService.CreateAndSendDailyAsync("Test Subject", "Preview Text", "Body Text", null));
        }
    }
}
