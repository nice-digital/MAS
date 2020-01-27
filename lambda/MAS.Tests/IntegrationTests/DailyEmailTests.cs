using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Fakes;
using MAS.Tests.Infrastructure;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public class DailyEmailTests : TestBase
    {
        public DailyEmailTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async void PuttingDailyEmailReturnsMailChimpCampaignInResponse()
        {
            // Arrange
            var client = _factory
               .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var responseText = await response.Content.ReadAsStringAsync();

            dynamic returnedCampaign = JObject.Parse(responseText);
            ((string)returnedCampaign.id).ShouldBe(FakeMailChimpManager.CampaignId);

            // TODO: Ideally we compare the *whole* campaign object, not just the id.
            // We could use on the options below however we can't because of this bug:
            // https://github.com/brandonseydel/MailChimp.Net/issues/447

            //var expectedJsonText = JsonConvert.SerializeObject(FakeMailChimpManager.Campaign);
            //responseText.ShouldBe(expectedJsonText);

            //var responseJson = JObject.Parse(responseText);
            //var expected = JObject.FromObject(FakeMailChimpManager.Campaign);
            //JObject.DeepEquals(responseJson, expected).ShouldBe(true);
        }

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
            await client.PutAsync("/api/mail/daily", null);

            // Assert
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
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "daily-items-single.json")
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
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "daily-items-{0}.json")
                .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily?date=01-01-2020", null);

            // Assert
            ((string)contentRequest.Template.Sections["body"]).ShouldMatchApproved();
        }

        [Fact]
        public async void CreatesMailChimpCampaignWithDynamicSegmentsToSendToCorrectUsers()
        {
            // Arrange
            Campaign campaign = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager.Setup(s => s.Campaigns.AddAsync(It.IsAny<Campaign>()))
                .Callback<Campaign>(c => campaign = c)
                .ReturnsAsync(FakeMailChimpManager.Campaign);

            var client = _factory
                .WithImplementation(fakeMailChimpManager.Object)
                .CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            campaign.Recipients.SegmentOptions.Match.ShouldBe(MailChimp.Net.Models.Match.Any);

            var conditions = campaign.Recipients.SegmentOptions.Conditions;
            conditions.Count().ShouldBe(2);

            var specialitiesCondition = conditions.First();
            var receiveEverythingCondition = conditions.Skip(1).Single();

            conditions.ShouldSatisfyAllConditions(
                // Check the specialities, based on data in feeds/daily-items.json
                () => specialitiesCondition.Type.ShouldBe(ConditionType.Interests),
                () => specialitiesCondition.Operator.ShouldBe(Operator.InterestContains),
                () => specialitiesCondition.Field.ShouldBe("interests-" + TestAppSettings.MailChimp.Default.SpecialityCategoryId),
                () => (specialitiesCondition.Value as string[]).ShouldBe(new string[] { "2", "5" }),
                // Check the receive everything dynamic segment
                () => receiveEverythingCondition.Type.ShouldBe(ConditionType.Interests),
                () => receiveEverythingCondition.Operator.ShouldBe(Operator.InterestContains),
                () => receiveEverythingCondition.Field.ShouldBe("interests-" + TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId),
                () => (receiveEverythingCondition.Value as string[]).ShouldBe(new string[] { FakeMailChimpManager.ReceiveEverythingGroupId })
                );
        }
    }
}
