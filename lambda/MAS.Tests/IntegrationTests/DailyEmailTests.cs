using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Moq;
using Shouldly;
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
            var fakeMailService = new Mock<IMailService>();

            string bodyHtml = string.Empty;
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((subject, previewText, body) => bodyHtml = body)
                .ReturnsAsync("1234");

            var client = _factory
                .WithImplementation(fakeMailService.Object)
                .CreateClient();

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
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((subject, previewText, body) => bodyHtml = body)
                .ReturnsAsync("1234");

            //CMSConfig cmsConfig = TestAppSettings.CMS.Default;
            //cmsConfig.DailyItemsPath = "/daily-items-single.json";

            var client = _factory
                .WithImplementation(fakeMailService.Object)
                //.WithCMSConfig(cmsConfig)
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "/daily-items-single.json")
                .CreateClient();

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
            fakeMailService.Setup(s => s.CreateAndSendDailyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((subject, previewText, body) => bodyHtml = body)
                .ReturnsAsync("1234");

            var client = _factory
                .WithImplementation(fakeMailService.Object)
                .WithCMSConfig(cmsConfig => cmsConfig.DailyItemsPath = "/daily-items-{0}.json")
                .CreateClient();

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
