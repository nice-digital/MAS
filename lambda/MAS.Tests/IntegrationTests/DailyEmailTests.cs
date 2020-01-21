using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Fakes;
using MAS.Tests.Infrastructure;
using Moq;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public class DailyEmailTests : TestBase
    {
        public DailyEmailTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async void EmailBodyHtmlMatchesApproved()
        {
            // Arrange
            ContentRequest contentRequest = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager
                .Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()))
                .Callback<string, ContentRequest>((cId, conReq) => contentRequest = conReq);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            responseText.ShouldBe("1234");

            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedForSingleItem()
        {
            // Arrange
            ContentRequest contentRequest = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager
                .Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()))
                .Callback<string, ContentRequest>((cId, conReq) => contentRequest = conReq);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "/daily-items-single.json")
                .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedForSingleItemWithSpecificDate()
        {
            // Arrange
            ContentRequest contentRequest = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager
                .Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()))
                .Callback<string, ContentRequest>((cId, conReq) => contentRequest = conReq);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "/daily-items-{0}.json")
                .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily?date=01-01-2020", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            responseText.ShouldBe("1234");

            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        //[Fact]
        //public async void SendsEmailToUsers()
        //{
        //    // Arrange
        //    Campaign campaign = null;
        //    var fakeMailChimpManager = new FakeMailChimpManager();
        //    fakeMailChimpManager.Setup(s => s.Campaigns.AddAsync(It.IsAny<Campaign>()))
        //        .Callback<Campaign>(c => campaign = c)
        //        .ReturnsAsync(new Campaign { Id = "1234" });

        //    var client = _factory
        //        .WithImplementation(fakeMailChimpManager.Object)
        //        .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "/daily-items-{0}.json")
        //        .CreateClient();

        //    // Act
        //    var response = await client.PutAsync("/api/mail/daily?date=01-01-2020", null);

        //    // Assert
        //    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        //    var responseText = await response.Content.ReadAsStringAsync();
        //    responseText.ShouldBe("1234");

        //    campaign.Recipients.SegmentOptions.Match.ShouldBe(MailChimp.Net.Models.Match.Any);
        //    // TODO: Check the conditions based on the test data
        //    campaign.Recipients.SegmentOptions.Conditions.ShouldBe();
        //}

    }
}
