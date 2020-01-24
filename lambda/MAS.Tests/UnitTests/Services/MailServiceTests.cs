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
using MAS.Tests.Fakes;
using System.Linq.Expressions;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests
    {
        [Fact]
        public async void CreateCampaignWithCorrectSettings()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            Campaign campaign = null;
            var fakeMailChimpManager = new FakeMailChimpManager();
            fakeMailChimpManager.Setup(s => s.Campaigns.AddAsync(It.IsAny<Campaign>()))
                .Callback<Campaign>(c => campaign = c)
                .ReturnsAsync(() => campaign);

            var mailConfig = new MailConfig
            {
                DailySubject = "Test - {0}",
                FromName = "Test from name",
                ReplyTo = "replyto@test.com"
            };

            var mailService = new MailService(fakeMailChimpManager.Object, mockLogger.Object, TestAppSettings.MailChimp.Default, mailConfig);

            var allSpecialities = FakeMailChimpManager.GetAllSpecialityInterests();

            var specialitiesInEmail = new List<string>() {
                allSpecialities.Skip(2).First().Name,
                allSpecialities.Skip(5).First().Name
            };

            //Act
            var response = await mailService.CreateAndSendDailyAsync(new DateTime(1815, 12, 10), "Preview Text", "Body Text", specialitiesInEmail, allSpecialities, FakeMailChimpManager.ReceiveEverythingGroupId);

            //Assert
            response.ShouldBe(campaign);

            campaign.Type.ShouldBe(CampaignType.Regular);

            var settings = campaign.Settings;
            settings.ShouldSatisfyAllConditions(
                () => settings.FolderId.ShouldBe(TestAppSettings.MailChimp.Default.CampaignFolderId),
                () => settings.TemplateId.ShouldBe(TestAppSettings.MailChimp.Default.DailyTemplateId),
                () => settings.SubjectLine.ShouldBe("Test - 10 December 1815"),
                () => settings.FromName.ShouldBe(mailConfig.FromName),
                () => settings.ReplyTo.ShouldBe(mailConfig.ReplyTo),
                () => settings.PreviewText.ShouldNotBeNullOrWhiteSpace(),
                () => settings.AutoFooter.ShouldBe(false)
            );

            var conditions = campaign.Recipients.SegmentOptions.Conditions;
            conditions.Count().ShouldBe(2);

            var specialitiesCondition = conditions.First();
            var receiveEverythingCondition = conditions.Skip(1).Single();

            conditions.ShouldSatisfyAllConditions(
                // Check the specialities, based on data in feeds/daily-items.json
                () => specialitiesCondition.Type.ShouldBe(ConditionType.Interests),
                () => specialitiesCondition.Operator.ShouldBe(Operator.InterestContains),
                () => specialitiesCondition.Field.ShouldBe("interests-" + TestAppSettings.MailChimp.Default.SpecialityCategoryId),
                () => (specialitiesCondition.Value as string[]).ShouldBe(new string[] { "3", "6" }),
                // Check the receive everything dynamic segment
                () => receiveEverythingCondition.Type.ShouldBe(ConditionType.Interests),
                () => receiveEverythingCondition.Operator.ShouldBe(Operator.InterestContains),
                () => receiveEverythingCondition.Field.ShouldBe("interests-" + TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId),
                () => (receiveEverythingCondition.Value as string[]).ShouldBe(new string[] { FakeMailChimpManager.ReceiveEverythingGroupId })
                );
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
            Should.Throw<Exception>(() => mailService.CreateAndSendDailyAsync(DateTime.Today, "Preview Text", "Body Text", new List<string>(), FakeMailChimpManager.GetAllSpecialityInterests(), FakeMailChimpManager.ReceiveEverythingGroupId));
        }
    }
}
