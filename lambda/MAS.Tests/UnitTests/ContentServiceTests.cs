using Amazon.S3;
using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Newtonsoft.Json;
using Shouldly;
using System.IO;
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
            var contentService = new ContentService(null);

            //Act
            var result = await contentService.GetItemAsync("");

            //Assert
            string actualOutputJson = JsonConvert.SerializeObject(result);
			//Ensure the json is formatted the same
            var expectedOutputObject = JsonConvert.DeserializeObject<Item>(File.ReadAllText("C:/_src/MAS/lambda/MAS.Tests/Feeds/single-item.json"));
            string expectedOutputJson = JsonConvert.SerializeObject(expectedOutputObject);

            actualOutputJson.ShouldBe(expectedOutputJson);
            

        }
    }
}
