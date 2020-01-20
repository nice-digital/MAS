using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Source = MAS.Models.Source;

namespace MAS.Tests.IntegrationTests
{
    public class TestMailService : IMailService
    {
        public Task<string> CreateAndSendDailyAsync(string subject, string previewText, string body, List<string> specialitiesInEmail)
        {
            throw new NotImplementedException();
        }
    }

    public class DailyEmailTests : TestBase
    {
        [Fact]
        public async void EmailBodyHtmlMatchesApproved()
        {
            // Arrange
            var fakeMailService = new Mock<IMailService>();

            string bodyHtml = string.Empty;
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, string, string, List<string>>((subject, previewText, body, specialitiesInEmail) => bodyHtml = body)
                .ReturnsAsync("1234");

            var client = WithImplementation(fakeMailService.Object).CreateClient();

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            responseText.ShouldBe("1234");

            bodyHtml.ShouldMatchApproved();
        }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedForSingleItem()
        {
            // Arrange
            var fakeMailService = new Mock<IMailService>();

            string bodyHtml = string.Empty;
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, string, string, List<string>>((subject, previewText, body, specialitiesInEmail) => bodyHtml = body)
                .ReturnsAsync("1234");

            var client = WithImplementation(fakeMailService.Object).CreateClient();

            AppSettings.CMSConfig.DailyItemsPath = "/daily-items-single.json";

            // Act
            var response = await client.PutAsync("/api/mail/daily", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            responseText.ShouldBe("1234");

            bodyHtml.ShouldMatchApproved();
        }

        [Fact]
        public async void EmailBodyHtmlMatchesApprovedForSingleItemWithSpecificDate()
        {
            // Arrange
            var fakeMailService = new Mock<IMailService>();

            string bodyHtml = string.Empty;
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, string, string, List<string>>((subject, previewText, body, specialitiesInEmail) => bodyHtml = body)
                .ReturnsAsync("1234");

            var client = WithImplementation(fakeMailService.Object).CreateClient();

            AppSettings.CMSConfig.DailyItemsPath = "/daily-items-{0}.json";

            // Act
            var response = await client.PutAsync("/api/mail/daily?date=01-01-2020", null);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            responseText.ShouldBe("1234");

            bodyHtml.ShouldMatchApproved();
        }

    }
}
