using MAS.Configuration;
using MAS.Controllers;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests.Controllers
{
    public class MailControllerTests
    {
        [Fact]
        public async void GetDailyItemsForToday()
        {

            var mockContentService = new Mock<IContentService>();
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), Mock.Of<ILogger<MailService>>(), Mock.Of<MailConfig>());

            //Act
            await mailService.PutMailAsync();

            //Assert
            mockContentService.Verify(mock => mock.GetDailyItemsAsync(null), Times.Once());
        }

        [Fact]
        public async void GetDailyItemsForSpecificDate()
        {
            DateTime date = new DateTime(2020, 1, 15);
            var mockContentService = new Mock<IContentService>();
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), Mock.Of<ILogger<MailService>>(), Mock.Of<MailConfig>());

            //Act
            await mailService.PutMailAsync(date);

            //Assert
            mockContentService.Verify(mock => mock.GetDailyItemsAsync(date), Times.Once());
        }

        [Fact]
        public async void DoesntRenderViewWhenNoItemsFound()
        {
            var items = Enumerable.Empty<Item>();
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetDailyItemsAsync(null)).ReturnsAsync(items);

            var mockViewRenderer = new Mock<IViewRenderer>();

            var mailConfig = new MailConfig
                {
                    DailySubject = ""
                };

            var mailController = new MailController(Mock.Of<IMailService>(), mockContentService.Object, mockViewRenderer.Object, Mock.Of<ILogger<MailService>>(), mailConfig);

            //Act
            await mailController.PutMailAsync();

            //Assert
            mockViewRenderer.Verify(mock => mock.RenderViewAsync(mailController, "~/Views/Mail/Daily.cshtml", items, false), Times.Never());
        }

        [Fact]
        public async void RendersDailyViewWithContentItems()
        {
            var items = new List<Item>() { new Item { } }.AsEnumerable();
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetDailyItemsAsync(null)).ReturnsAsync(items);

            var mockViewRenderer = new Mock<IViewRenderer>();

            var mailConfig = new MailConfig
            {
                DailySubject = ""
            };

            var mailController = new MailController(Mock.Of<IMailService>(), mockContentService.Object, mockViewRenderer.Object, Mock.Of<ILogger<MailService>>(), mailConfig);

            //Act
            await mailController.PutMailAsync();

            //Assert
            mockViewRenderer.Verify(mock => mock.RenderViewAsync(mailController, "~/Views/Mail/Daily.cshtml", items, false), Times.Once());
        }

        [Fact]
        public async void SendsSubjectAndBodyToMailService()
        {
            var items = new List<Item>() { new Item { Specialities = new List<Speciality>() } }.AsEnumerable();
            var mockContentService = new Mock<IContentService>();
            mockContentService
                .Setup(x => x.GetDailyItemsAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(items);

            var body = "<p>body</p>";
            var mockViewRenderer = new Mock<IViewRenderer>();
            mockViewRenderer
                .Setup(x => x.RenderViewAsync(It.IsAny<MailController>(), It.IsAny<string>(), It.IsAny<IEnumerable<Item>>(), false))
                .ReturnsAsync(body);

            var mockMailService = new Mock<IMailService>();

            var mailConfig = new MailConfig
            {
                DailySubject = "Test subject - {0}"
            };

            var mailController = new MailController(mockMailService.Object, mockContentService.Object, mockViewRenderer.Object, Mock.Of<ILogger<MailService>>(), mailConfig);

            //Act
            await mailController.PutMailAsync(new DateTime(2020, 1, 15));

            //Assert
            mockMailService.Verify(mock => mock.CreateAndSendDailyAsync("Test subject - 15 January 2020", It.IsAny<string>(), body, It.IsAny<List<string>>()), Times.Once());
        }
    }
}
