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
using System;
using System.Linq;
using System.Collections.Generic;
using MAS.Tests.Fakes;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests
    {
        [Fact]
        public void CreateCampaignAndSendToMailChimp()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            var fakeMailChimpManager = new FakeMailChimpManager();

            var mailService = new MailService(fakeMailChimpManager.Object, mockLogger.Object, Mock.Of<MailChimpConfig>(), Mock.Of<MailConfig>());

            //Act
            var response = mailService.CreateAndSendDailyAsync("Test Subject", "Preview Text", "Body Text", new List<string>(), FakeMailChimpManager.GetAllSpecialityInterests(), FakeMailChimpManager.ReceiveEverythingGroupId);
            
            //Assert
            response.Exception.ShouldBe(null);
            response.Result.ShouldBe(FakeMailChimpManager.Campaign);
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
            Should.Throw<Exception>(() => mailService.CreateAndSendDailyAsync("Test Subject", "Preview Text", "Body Text", new List<string>(), FakeMailChimpManager.GetAllSpecialityInterests(), FakeMailChimpManager.ReceiveEverythingGroupId));
        }
    }
}
