using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Source = MAS.Models.Source;

namespace MAS.Tests.UnitTests
{
    public class DailyEmailTests : TestBase
    {

        Mock<ILogger<MailService>> MockLogger;
        Mock<IMailChimpManager> MockMailChimpManager;
        MailService MailService;

        public DailyEmailTests()
        {
            //Arrange
            MockLogger = new Mock<ILogger<MailService>>();

            MockMailChimpManager = new Mock<IMailChimpManager>();
            MockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            MockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            MockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>()));

            MailService = new MailService(MockMailChimpManager.Object, MockLogger.Object);
        }

        public override void Dispose()
        {

        }

        Item exampleItem = new Item()
        {
            Id = "123",
            Title = "Some Title",
            Slug = "abc",
            ShortSummary = "Some short summary",
            ResourceLinks = "",
            Comment = "",
            Specialities = new List<Speciality>
                {
                    new Speciality()
                    {
                        Key = "1a",
                        Title = "Some speciality",

                    }
                },
            EvidenceType = new EvidenceType()
            {
                Key = "1b",
                Title = "Some evidence type"
            },
            Source = new Source()
            {
                Id = "1c",
                Title = "Some source"
            }
        };
        Item exampleItem2 = new Item()
        {
            Id = "123",
            Title = "Some Title",
            Slug = "abc",
            ShortSummary = "Some short summary",
            ResourceLinks = "",
            Comment = "",
            Specialities = new List<Speciality>
            {
                    new Speciality()
                    {
                        Key = "1a",
                        Title = "Some speciality 2",

                    }
            },
            EvidenceType = new EvidenceType()
            {
                Key = "1c",
                Title = "Some evidence type 2"
            },
            Source = new Source()
            {
                Id = "1c",
                Title = "Some source"
            }
        };

        [Fact]
        public void CanCreateSingleItemEmail()
        {
            var items = new List<Item> { exampleItem };
            var actualHtml = this.MailService.CreateDailyEmailBody(items);

            actualHtml.ShouldMatchApproved();

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsSharingEvidenceType()
        {
            var items = new List<Item> { exampleItem, exampleItem };
            var actualHtml = this.MailService.CreateDailyEmailBody(items);

            actualHtml.ShouldMatchApproved();

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsDifferentEvidenceType()
        {
            var items = new List<Item> { exampleItem, exampleItem2 };
            var actualHtml = this.MailService.CreateDailyEmailBody(items);

            actualHtml.ShouldMatchApproved();

        }

        [Fact]
        public void ItemsWithManySpecialitiesRenderCorrectly()
        {
            exampleItem.Specialities.Add(new Speciality() { Key = "abcd", Title = "Another speciality" });
            var items = new List<Item> { exampleItem };
            var actualHtml = this.MailService.CreateDailyEmailBody(items);

            actualHtml.ShouldMatchApproved();

        }

    }
}
