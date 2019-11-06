using Amazon.S3;
using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using System;

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
            string actualJson = JsonConvert.SerializeObject(result);
            actualJson.ShouldMatchApproved();
            
        }
    }
}
