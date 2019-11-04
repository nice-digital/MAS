using Amazon.S3;
using Amazon.S3.Model;
using MAS.Services;
using MAS.Models;
using MAS.Tests.Infrastructure;
using Moq;
using System.Net;
using System.Threading;
using Xunit;
using Shouldly;

namespace MAS.Tests.IntergrationTests
{
    public class S3ServiceTests : TestBase
    {
        [Fact]
        public void WriteToS3()
        {
            //Arrange
            var mockAmazonS3 = new Mock<IAmazonS3>();
            mockAmazonS3.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            var s3Service = new S3Service(mockAmazonS3.Object);

            //Act
            var response = s3Service.WriteToS3(new Item { Id = "1234", Title = "My Test Drug" });

            //Assert
            response.Result.HttpStatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
;