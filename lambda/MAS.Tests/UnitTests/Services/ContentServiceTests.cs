using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class ContentServiceTests
    {
        [Fact]
        public async Task ReadMultipleItems()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<ContentService>>();

            AppSettings.CMSConfig = TestAppSettings.CMS.Default;
            var contentService = new ContentService(mockLogger.Object);

            //Act
            var result = await contentService.GetAllItemsAsync();

            //Assert
            result.Count().ShouldBe(4);
            result.FirstOrDefault().Id.ShouldBe("5daf1aa18a34d4bb8405b5e0");
            result.FirstOrDefault().Title.ShouldBe("Wonder Drug");
            result.LastOrDefault().Id.ShouldBe("5db6cbcc8a34d4ca5905b5e4");
            result.LastOrDefault().Title.ShouldBe("A Placebo");
        }

        [Fact]
        public async Task InvalidURIThrowsError()
        { 
            //Arrange
            var mockLogger = new Mock<ILogger<ContentService>>();

            AppSettings.CMSConfig = TestAppSettings.CMS.InvalidURI;
            var contentService = new ContentService(mockLogger.Object);

            //Act + Assert
            await Should.ThrowAsync<Exception>(() => contentService.GetAllItemsAsync());
        }
    }
}
