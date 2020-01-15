using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Source = MAS.Models.Source;

namespace MAS.Tests.IntergrationTests
{
    public class FakeController : Microsoft.AspNetCore.Mvc.Controller
    {

    }
    
    public class DailyEmailTests : TestBase
    {
        IMailService MailService;

        public DailyEmailTests(IMailService mailService)
        {

            MailService = mailService;
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

        //[Fact]
        //public void CanCreateSingleItemEmail()
        //{

        //    var items = new List<Item> { exampleItem };
        //    var actualHtml = this.MailService.CreateDailyEmailBody(items, mockController.Object);

        //    actualHtml.ShouldMatchApproved();

        //}

        //[Fact]
        //public void CanCreateEmailWithTwoItemsSharingEvidenceType()
        //{
        //    var items = new List<Item> { exampleItem, exampleItem };
        //    var actualHtml = this.MailService.CreateDailyEmailBody(items, new FakeController());

        //    actualHtml.ShouldMatchApproved();

        //}

        //[Fact]
        //public void CanCreateEmailWithTwoItemsDifferentEvidenceType()
        //{
        //    var items = new List<Item> { exampleItem, exampleItem2 };
        //    var actualHtml = "";
        //    try
        //    {
        //        actualHtml = this.MailService.CreateDailyEmailBody(items, new FakeController());
        //    }
        //    catch(Exception e)
        //    {
        //        var p = e.Message;
        //    }
            

        //    actualHtml.ShouldMatchApproved();

        //}

        //[Fact]
        //public void ItemsWithManySpecialitiesRenderCorrectly()
        //{
        //    exampleItem.Specialities.Add(new Speciality() { Key = "abcd", Title = "Another speciality" });
        //    var items = new List<Item> { exampleItem };
        //    var actualHtml = this.MailService.CreateDailyEmailBody(items, new FakeController());

        //    actualHtml.ShouldMatchApproved();

        //}

    }
}
