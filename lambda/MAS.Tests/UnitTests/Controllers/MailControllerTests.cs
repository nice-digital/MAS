using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Controllers;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Fakes;
using MAS.Tests.Infrastructure;
using MAS.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MAS.Tests.UnitTests.Controllers
{
    public class MailControllerTests
    {
        [Fact]
        public async void GetDailyItemsForSpecificDate()
        {
            DateTime date = new DateTime(2020, 1, 15);
            var mockContentService = new Mock<IContentService>();
            var mailService = new MailController(new FakeMailChimpManager().Object, Mock.Of<IMailService>(), mockContentService.Object, Mock.Of<IViewRenderer>(), new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>(), Mock.Of<MailConfig>(), TestAppSettings.MailChimp.Default);

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
            mockContentService.Setup(x => x.GetDailyItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

            var mockViewRenderer = new Mock<IViewRenderer>();

            var mailConfig = new MailConfig
                {
                    DailySubject = ""
                };

            var mailController = new MailController(new FakeMailChimpManager().Object, Mock.Of<IMailService>(), mockContentService.Object, mockViewRenderer.Object, new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>(), mailConfig, TestAppSettings.MailChimp.Default);
            
            //Act
            await mailController.PutMailAsync();

            //Assert
            mockViewRenderer.Verify(mock => mock.RenderViewAsync(mailController, "~/Views/Mail/Daily.cshtml", items, false), Times.Never());
        }

        [Fact]
        public async void RendersDailyViewWithViewModel()
        {
            var items = new List<Item>() { new Item { } }.AsEnumerable();
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetDailyItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

            var mockViewRenderer = new Mock<IViewRenderer>();

            var mailConfig = new MailConfig
            {
                DailySubject = ""
            };

            var mailController = new MailController(new FakeMailChimpManager().Object, Mock.Of<IMailService>(), mockContentService.Object, mockViewRenderer.Object, new FakeBankHolidayService(), Mock.Of<ILogger<MailController>>(), mailConfig, TestAppSettings.MailChimp.Default);
            
            //Act
            await mailController.PutMailAsync();

            //Assert
            // TODO: Assert on the actual view model
            mockViewRenderer.Verify(mock => mock.RenderViewAsync(mailController, "~/Views/Mail/Daily.cshtml", It.IsAny<DailyEmailViewModel>(), false), Times.Once());
        }

        [Fact]
        public async void SendsDateAndBodyToMailService()
        {
            var items = new List<Item>() { new Item { Specialities = new List<Speciality>() } }.AsEnumerable();
            var mockContentService = new Mock<IContentService>();
            mockContentService
                .Setup(x => x.GetDailyItemsAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(items);

            var body = "<p>body</p>";
            var mockViewRenderer = new Mock<IViewRenderer>();
            mockViewRenderer
                .Setup(x => x.RenderViewAsync(It.IsAny<MailController>(), It.IsAny<string>(), It.IsAny<DailyEmailViewModel>(), false))
                .ReturnsAsync(body);

            var mockMailService = new Mock<IMailService>();

            var mailController = new MailController(new FakeMailChimpManager().Object, mockMailService.Object, mockContentService.Object, mockViewRenderer.Object, Mock.Of<IBankHolidayService>(), Mock.Of<ILogger<MailController>>(), Mock.Of<MailConfig>(), TestAppSettings.MailChimp.Default);
            var sendDate = new DateTime(1856, 7, 10);

            //Act
            await mailController.PutMailAsync(sendDate);

            //Assert
            mockMailService.Verify(mock => mock.CreateAndSendDailyAsync(sendDate, It.IsAny<string>(), body, It.IsAny<List<string>>(), It.IsAny<IEnumerable<Interest>>(), It.IsAny<string>()), Times.Once());
        }
    }
}
