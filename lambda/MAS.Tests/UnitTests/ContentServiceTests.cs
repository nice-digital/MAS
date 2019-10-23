using Amazon.S3;
using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class ContentServiceTests : TestBase
    {
        public ContentServiceTests()
        {
            AppSettings.CMSConfig = TestAppSettings.GetCMSConfig();
        }

        [Fact]
        public async Task Get_ItemAsync()
        {
            //Arrange
            var contentService = new ContentService();

            //Act
            var result = await contentService.GetItemAsync("");

            //Assert
            result.Id.ShouldBe("5daeb5af22565a82530d7373"); 
            result.Title.ShouldBe("Wonder drug");
        }
    }
}
