using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests
{
    public class ContentServerTests : TestBase
    {
        public ContentServerTests()
        {
            AppSettings.CMSConfig = TestAppSettings.GetCMSConfig();
        }

        [Fact]
        public async Task Get_ItemAsync()
        {
            //Arrange
            var contentSerivce = new ContentService();

            //Act
            var result = await contentSerivce.GetItemAsync("");

            //Assert
            result.Id.ShouldBe("5daeb5af22565a82530d7373"); 
            result.Title.ShouldBe("Wonder drug");
        }
    }
}
