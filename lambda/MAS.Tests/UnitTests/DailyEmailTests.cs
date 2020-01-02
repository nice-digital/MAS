using MAS.Models;
using MAS.Tests.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class DailyEmailTests : TestBase
    {
        Item exampleItem = new Item()
        {
            Id = "123",
            Title = "Some Title",
            Slug = "abc",
            ShortSummary = "Some short summary",
            ResourceLinks = "",
            Comment = "",
            Speciality = new Speciality[]
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
            Speciality = new Speciality[]
            {
                    new Speciality()
                    {
                        Key = "1a",
                        Title = "Some speciality 2",

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

        [Fact]
        public void CanCreateSingleItemEmail()
        {
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem
                }
            };
        
            var expectedHtml = "<strong>Some speciality</strong><br>Some source<br>Some Title<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a>";
            email.HTML.ShouldBe(expectedHtml);

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsSharingASpeciality()
        {
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem,
                    exampleItem
                }
            };

            var expectedHtml = "<strong>Some speciality</strong><br>Some source<br>Some Title<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a><br>Some source<br>Some Title<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a>";
            email.HTML.ShouldBe(expectedHtml);

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsdifferentSpecialities()
        {
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem,
                    exampleItem2
                }
            };

            var expectedHtml = "<strong>Some speciality</strong><br>Some source<br>Some Title<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a><strong>Some speciality 2</strong><br>Some source<br>Some Title<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a>";
            email.HTML.ShouldBe(expectedHtml);

        }
    }
}
