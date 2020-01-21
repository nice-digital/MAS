using MAS.Controllers;
using MAS.Models;
using MAS.Services;
using Microsoft.Extensions.Logging;
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
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), Mock.Of<ILogger<MailService>>());

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
            var mailService = new MailController(Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), Mock.Of<ILogger<MailService>>());

            //Act
            await mailService.PutMailAsync(date);

            //Assert
            mockContentService.Verify(mock => mock.GetDailyItemsAsync(date), Times.Once());
        }

        [Fact]
        public async void RendersDailyViewWithContentItems()
        {
            var items = new List<Item>() { }.AsEnumerable();
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetDailyItemsAsync(null)).ReturnsAsync(items);

            var mockViewRenderer = new Mock<IViewRenderer>();

            var mailController = new MailController(Mock.Of<IMailService>(), mockContentService.Object, mockViewRenderer.Object, Mock.Of<ILogger<MailService>>());

            //Act
            await mailController.PutMailAsync();

            //Assert
            mockViewRenderer.Verify(mock => mock.RenderViewAsync(mailController, "~/Views/DailyEmail.cshtml", items, false), Times.Once());
        }
    }
}
