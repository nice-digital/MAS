using System.Threading.Tasks;
using Xunit;
using Shouldly;
using MAS.Tests.Infrastructure;
using System.Text;
using System;
using MAS.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using MailChimp.Net.Models;
using Newtonsoft.Json;
using MailChimp.Net.Interfaces;
using Moq;
using MAS.Services;
using MAS.Controllers;
using Microsoft.Extensions.Logging;

namespace MAS.Tests.IntergrationTests.Mail
{
    public class MailControllerTests : TestBase
    {
        //[Fact]
        //public async Task PutRequestCreatesAndSendsDailyCampaign()
        //{
        //    //Arrange
        //    const string mailChimpCampaignsURI = "https://us5.api.mailchimp.com/3.0/campaigns/";
        //    AppSettings.CMSConfig = TestAppSettings.GetMultipleItemsFeed();

        //    var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"anystring:{AppSettings.MailChimpConfig.ApiKey}")));
        //    var client = new HttpClient()
        //    {
        //        DefaultRequestHeaders = { Authorization = authValue }
        //    };

        //    //Act
        //    var response = await _client.PutAsync("/api/mail/daily", null);

        //    //Get campaign to check if it saved
        //    var campaignId = await response.Content.ReadAsStringAsync();
        //    var campaign = await client.GetAsync(mailChimpCampaignsURI + campaignId);
        //    var campaignJson = await campaign.Content.ReadAsStringAsync();
        //    var campaignResult = JsonConvert.DeserializeObject<Campaign>(campaignJson);

        //    // Assert
        //    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        //    campaignResult.Status.ShouldNotBeNull();
        //    campaignResult.Status.ShouldNotBe("Draft");
        //}

        //[Fact]
        //public async Task PutRequestCreatesAndSendsWeeklyCampaign()
        //{
        //    //Arrange
        //    const string mailChimpCampaignsURI = "https://us5.api.mailchimp.com/3.0/campaigns/";
        //    AppSettings.CMSConfig = TestAppSettings.GetWeeklyFeed();

        //    var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"anystring:{AppSettings.MailChimpConfig.ApiKey}")));
        //    var client = new HttpClient()
        //    {
        //        DefaultRequestHeaders = { Authorization = authValue }
        //    };

        //    //Act
        //    var response = await _client.PutAsync("/api/mail/weekly", null);

        //    //Get campaign to check if it saved
        //    var campaignId = await response.Content.ReadAsStringAsync();
        //    var campaign = await client.GetAsync(mailChimpCampaignsURI + campaignId);
        //    var campaignJson = await campaign.Content.ReadAsStringAsync();
        //    var campaignResult = JsonConvert.DeserializeObject<Campaign>(campaignJson);

        //    // Assert
        //    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        //    campaignResult.Status.ShouldNotBeNull();
        //    campaignResult.Status.ShouldNotBe("Draft");
        //}

        [Fact]
        public async Task GivenNormalMonday_SendEmail()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>());

            var date = new DateTime(2020, 1, 13);

            //Act
            await mailService.PutWeeklyMailAsync(date);

            // Assert
            mockContentService.Verify(mock => mock.GetWeeklyAsync(date), Times.Once());
        }

        [Fact]
        public async Task GivenTuesdayAndPreviousMondayWasNotBankHoliday_EmailWasAlreadySent()
        {
            //Arrange

            //Act
            var response = await _client.PutAsync("/api/mail/weekly/2020-01-14", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            responseContent.ShouldBe("An email was sent on 13/01/2020");
        }

        [Fact]
        public async Task GivenMondayIsBankHoliday_DontSendEmail()
        {
            //Arrange

            //Act
            var response = await _client.PutAsync("/api/mail/weekly/2019-08-26", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            responseContent.ShouldBe("26/08/2019 is a bank holiday therefore an email isnt sent");
        }

        [Fact]
        public async Task GivenTuesdayAndPreviousMondayWasBankHoliday_SendEmail()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>());

            var date = new DateTime(2019, 8, 27);
            var previousMonday = new DateTime(2019, 8, 26, 00, 00, 00);

            //Act
            await mailService.PutWeeklyMailAsync(date);

            // Assert
            mockContentService.Verify(mock => mock.GetWeeklyAsync(previousMonday), Times.Once());
        }
    }
}
