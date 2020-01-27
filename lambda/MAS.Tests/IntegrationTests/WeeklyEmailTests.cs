using MailChimp.Net.Core;
using MAS.Configuration;
using MAS.Controllers;
using MAS.Services;
using MAS.Tests.Fakes;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public class WeeklyEmailTests : TestBase
    {
        public WeeklyEmailTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedWithMEC()
        {
            // Arrange
            ContentRequest contentRequest = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager
                .Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()))
                .Callback<string, ContentRequest>((cId, conReq) => contentRequest = conReq);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .WithCMSConfig(cmsConfig => cmsConfig.WeekliesBySendDatePath = "/weekly.json")
                .CreateClient();

            // Act
            await client.PutAsync("/api/mail/weekly/2020-01-13", null);

            // Assert
            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedWithoutMEC()
        {
            // Arrange
            ContentRequest contentRequest = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager
                .Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()))
                .Callback<string, ContentRequest>((cId, conReq) => contentRequest = conReq);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .WithCMSConfig(cmsConfig => cmsConfig.WeekliesBySendDatePath = "/weekly-without-mec.json")
                .CreateClient();

            // Act
            await client.PutAsync("/api/mail/weekly/2020-01-13", null);

            // Assert
            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        [Fact]
        public async Task GivenNormalMonday_SendEmail()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            var mailController = new MailController(new FakeMailChimpManager().Object, Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>(), Mock.Of<MailConfig>(), TestAppSettings.MailChimp.Default);

            var date = new DateTime(2020, 1, 13);

            //Act
            await mailController.PutWeeklyMailAsync(date);

            // Assert
            mockContentService.Verify(mock => mock.GetWeeklyAsync(date), Times.Once());
        }

        [Fact]
        public async Task GivenTuesdayAndPreviousMondayWasNotBankHoliday_EmailWasAlreadySent()
        {
            //Arrange
            var client = _factory
              .CreateClient();

            //Act
            var response = await client.PutAsync("/api/mail/weekly/2020-01-14", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            responseContent.ShouldBe("An email was sent on 13/01/2020");
        }

        [Fact]
        public async Task GivenMondayIsBankHoliday_DontSendEmail()
        {
            //Arrange
            var client = _factory
              .CreateClient();

            //Act
            var response = await client.PutAsync("/api/mail/weekly/2019-08-26", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            responseContent.ShouldBe("26/08/2019 is a bank holiday therefore an email isnt sent");
        }

        [Fact]
        public async Task GivenTuesdayAndPreviousMondayWasBankHoliday_SendEmail()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            var mailController = new MailController(new FakeMailChimpManager().Object, Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>(), Mock.Of<MailConfig>(), TestAppSettings.MailChimp.Default);

            var date = new DateTime(2019, 8, 27);
            var previousMonday = new DateTime(2019, 8, 26, 00, 00, 00);

            //Act
            await mailController.PutWeeklyMailAsync(date);

            // Assert
            mockContentService.Verify(mock => mock.GetWeeklyAsync(previousMonday), Times.Once());
        }
    }
}
