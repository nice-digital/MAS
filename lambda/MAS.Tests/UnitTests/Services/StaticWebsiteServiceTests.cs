using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests.Services
{
    public class StaticWebsiteServiceTests
    {
        [Fact]
        public async Task CanSuccessfullyWriteFilesAndInvalidateCache()
        {
            //Arrange
            var mockS3 = new Mock<IAmazonS3>();
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();
            mockCloudFrontService.Setup(cf => cf.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)))
                .ReturnsAsync(new CreateInvalidationResponse { HttpStatusCode = HttpStatusCode.Created });

            var cfConfig = new CloudFrontConfig() { Enabled = "true" };
            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, Mock.Of<ILogger<S3StaticWebsiteService>>(), Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), cfConfig);

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };
            var filePaths = new List<string>() { "/" + a.FilePath, "/" + b.FilePath };

            //Act
            var result = service.WriteFilesAsync(a,b).Result;

            //Assert
            mockCloudFrontService.Verify(x => x.CreateInvalidationAsync(It.Is<CreateInvalidationRequest>(
                o => Enumerable.SequenceEqual(o.InvalidationBatch.Paths.Items, filePaths)), default(CancellationToken)), Times.Once);
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == a.FilePath), default(CancellationToken)), Times.Exactly(1));
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == b.FilePath), default(CancellationToken)), Times.Exactly(1));
            Assert.Equal(HttpStatusCode.Created, result);
        }

        [Fact]
        public async Task ReturnErrorCodeIfCacheInvalidationFails()
        {
            //Arrange
            var mockS3 = new Mock<IAmazonS3>();
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();
            mockCloudFrontService.Setup(cf => cf.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)))
                .ReturnsAsync(new CreateInvalidationResponse { HttpStatusCode = HttpStatusCode.InternalServerError });

            var cfConfig = new CloudFrontConfig() { Enabled = "true" };
            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, Mock.Of<ILogger<S3StaticWebsiteService>>(), Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), cfConfig);

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };
            var filePaths = new List<string>() { "/" + a.FilePath, "/" + b.FilePath };

            //Act
            var result = service.WriteFilesAsync(a, b).Result;

            //Assert
            mockCloudFrontService.Verify(x => x.CreateInvalidationAsync(It.Is<CreateInvalidationRequest>(
                o => Enumerable.SequenceEqual(o.InvalidationBatch.Paths.Items, filePaths)), default(CancellationToken)), Times.Once);
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == a.FilePath), default(CancellationToken)), Times.Exactly(1));
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == b.FilePath), default(CancellationToken)), Times.Exactly(1));
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public async Task ReturnsErrorCodeWhenWriteFails()
        {
            //Arrange
            var mockS3 = new Mock<IAmazonS3>();
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.InternalServerError });

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();

            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, Mock.Of<ILogger<S3StaticWebsiteService>>(), Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), Mock.Of<CloudFrontConfig>());

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };
            var filePaths = new List<string>() { "/" + a.FilePath, "/" + b.FilePath };

            //Act
            var result = service.WriteFilesAsync(a, b).Result;

            //Assert
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == a.FilePath), default(CancellationToken)), Times.Exactly(1));
            mockS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(o => o.Key == b.FilePath), default(CancellationToken)), Times.Exactly(1));
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }
    }
}
