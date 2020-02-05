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
            var mockLogger = new Mock<ILogger<S3StaticWebsiteService>>();

            var mockS3 = new Mock<IAmazonS3>();
            var mockS3Resposne = new Mock<PutObjectResponse>();
            mockS3Resposne.Object.HttpStatusCode = HttpStatusCode.OK;
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(mockS3Resposne.Object);

            var mockCfResposne = new Mock<CreateInvalidationResponse>();
            mockCfResposne.Object.HttpStatusCode = HttpStatusCode.Created;

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();
            mockCloudFrontService.Setup(cf => cf.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)))
                .ReturnsAsync(mockCfResposne.Object);

            var cfConfig = new CloudFrontConfig() { Enabled = "true" };
            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, mockLogger.Object, Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), cfConfig);

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };

            var result = service.WriteFilesAsync(a,b).Result;

            mockCloudFrontService.Verify(x => x.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)), Times.Once);
            mockS3.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)), Times.Exactly(2));
            Assert.Equal(HttpStatusCode.Created, result);
        }

        [Fact]
        public async Task ReturnErrorCodeIfCacheInvalidationFails()
        {
            var mockLogger = new Mock<ILogger<S3StaticWebsiteService>>();

            var mockS3 = new Mock<IAmazonS3>();
            var mockS3Resposne = new Mock<PutObjectResponse>();
            mockS3Resposne.Object.HttpStatusCode = HttpStatusCode.OK;
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(mockS3Resposne.Object);

            var mockCfResposne = new Mock<CreateInvalidationResponse>();
            mockCfResposne.Object.HttpStatusCode = HttpStatusCode.InternalServerError;

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();
            mockCloudFrontService.Setup(cf => cf.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)))
                .ReturnsAsync(mockCfResposne.Object);

            var cfConfig = new CloudFrontConfig() { Enabled = "true" };
            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, mockLogger.Object, Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), cfConfig);

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };

            var result = service.WriteFilesAsync(a, b).Result;

            mockCloudFrontService.Verify(x => x.CreateInvalidationAsync(It.IsAny<CreateInvalidationRequest>(), default(CancellationToken)), Times.Once);
            mockS3.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)), Times.Exactly(2));
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public async Task ReturnsErrorCodeWhenWriteFails()
        {
            var mockLogger = new Mock<ILogger<S3StaticWebsiteService>>();

            var mockS3 = new Mock<IAmazonS3>();
            var mockS3Resposne = new Mock<PutObjectResponse>();
            mockS3Resposne.Object.HttpStatusCode = HttpStatusCode.InternalServerError;
            mockS3.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .ReturnsAsync(mockS3Resposne.Object);

            var mockCloudFrontService = new Mock<IAmazonCloudFront>();

            var service = new S3StaticWebsiteService(mockS3.Object, mockCloudFrontService.Object, mockLogger.Object, Mock.Of<AWSConfig>(), Mock.Of<EnvironmentConfig>(), Mock.Of<CloudFrontConfig>());

            var a = new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = new System.IO.MemoryStream() };
            var b = new StaticContentRequest { FilePath = "abc.html", ContentBody = "Some html" };

            var result = service.WriteFilesAsync(a, b).Result;
            
            mockS3.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)), Times.Exactly(2));
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }
    }
}
