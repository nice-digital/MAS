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
            Speciality = new List<Speciality>
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
            Speciality = new List<Speciality>
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
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem
                }
            };
        
            var expectedHtml = "<div class='evidenceType'><strong>Some evidence type</strong><div class='item'>Some Title<br>Some source<br>Some speciality<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div></div>";
            email.HTML.ShouldBe(expectedHtml);

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsSharingEvidenceType()
        {
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem,
                    exampleItem
                }
            };

            var expectedHtml = "<div class='evidenceType'><strong>Some evidence type</strong><div class='item'>Some Title<br>Some source<br>Some speciality<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div><div class='item'>Some Title<br>Some source<br>Some speciality<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div></div>";
            email.HTML.ShouldBe(expectedHtml);

        }

        [Fact]
        public void CanCreateEmailWithTwoItemsDifferentEvidenceType()
        {
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem,
                    exampleItem2
                }
            };

            var expectedHtml = "<div class='evidenceType'><strong>Some evidence type</strong><div class='item'>Some Title<br>Some source<br>Some speciality<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div></div><div class='evidenceType'><strong>Some evidence type 2</strong><div class='item'>Some Title<br>Some source<br>Some speciality 2<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div></div>";
            email.HTML.ShouldBe(expectedHtml);

        }

        [Fact]
        public void ItemsWithManySpecialitiesRenderCorrectly()
        {
            exampleItem.Speciality.Add(new Speciality() { Key = "abcd", Title = "Another speciality" });
            var email = new DailyEmail()
            {
                Items = new List<Item>()
                {
                    exampleItem
                }
            };

            var expectedHtml = "<div class='evidenceType'><strong>Some evidence type</strong><div class='item'>Some Title<br>Some source<br>Some speciality | Another speciality<br>Some short summary<br><a href='https://www.medicinesresources.nhs.uk/abc'>SPS Comment</a></div></div>";
            email.HTML.ShouldBe(expectedHtml);

        }
    }
}
