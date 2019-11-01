using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using MAS.Tests.Infrastructure;
using System.Net.Http;
using System.Text;
using System.Net;
using Shouldly;
using MAS.Services;
using Moq;
using MAS.Models;
using MAS.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace MAS.Tests.IntergrationTests
{
    public class ContentControllerTests : TestBase
    {
        [Fact]
        public async Task Put()
        {
            //Arrange
            var mockContentService = new Mock<IContentService>();
            mockContentService.Setup(x => x.GetItemAsync(It.IsAny<string>())).ReturnsAsync(JsonConvert.DeserializeObject<Item>("{ \"Id\" : \"1234\", \"Title\" : \"My Test Drug\" }"));

            var mockS3Service = new Mock<IS3Service>();
            mockS3Service.Setup(x => x.WriteToS3(It.IsAny<Item>())).ReturnsAsync(new Amazon.S3.Model.PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            var mockLogger = new Mock<ILogger<ContentController>>();

            var content = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
            var contentController = new ContentController(mockContentService.Object, mockS3Service.Object, mockLogger.Object);

            //Act
            var response = await contentController.PutAsync("/api/content/1234") as Microsoft.AspNetCore.Mvc.StatusCodeResult;

            // Assert
            response.StatusCode.ShouldBe(200);
        }
    }
}
