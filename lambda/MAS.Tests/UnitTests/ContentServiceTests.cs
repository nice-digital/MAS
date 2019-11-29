using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class ContentServiceTests : TestBase
    {
        [Fact]
        public async Task ReadMultipleItems()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<ContentService>>();

            AppSettings.CMSConfig = TestAppSettings.GetMultipleItemsFeed();
            var contentService = new ContentService(mockLogger.Object);

            //Act
            var result = await contentService.GetItemsAsync();

            //Assert
            result.Count().ShouldBe(4);
            result.FirstOrDefault().Id.ShouldBe("5daf1aa18a34d4bb8405b5e0");
            result.FirstOrDefault().Title.ShouldBe("Wonder Drug");
            result.LastOrDefault().Id.ShouldBe("5db6cbcc8a34d4ca5905b5e4");
            result.LastOrDefault().Title.ShouldBe("A Placebo");
        }
    }
}
