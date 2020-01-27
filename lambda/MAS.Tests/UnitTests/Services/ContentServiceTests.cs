using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class ContentServiceTests
    {
        [Fact]
        public async Task ReadAllItems()
        {
            //Arrange
             var contentService = new ContentService(Mock.Of<ILogger<ContentService>>(), TestAppSettings.CMS.Default);

            //Act
            var result = await contentService.GetAllItemsAsync();

            //Assert
            result.Count().ShouldBe(4);

            result.First().Id.ShouldBe("5daf1aa18a34d4bb8405b5e0");
            result.First().Slug.ShouldBe("wonder-drug");
            result.First().Title.ShouldBe("Wonder Drug");

            result.Last().Id.ShouldBe("5db6cbcc8a34d4ca5905b5e4");
            result.Last().Slug.ShouldBe("a-placebo");
            result.Last().Title.ShouldBe("A Placebo");
        }

        [Fact]
        public async Task InvalidJsonResponseForAllItemsThrowsError()
        {
            //Arrange
            var cmsConfig = TestAppSettings.CMS.Default;
            cmsConfig.AllItemsPath = "all-items-invalid.json";
            var contentService = new ContentService(Mock.Of<ILogger<ContentService>>(), cmsConfig);

            //Act + Assert
            var exception = await Should.ThrowAsync<Exception>(() => contentService.GetAllItemsAsync());
            exception.InnerException.ShouldBeOfType<JsonSerializationException>();
        }

        [Fact]
        public async Task InvalidURIThrowsError()
        { 
            //Arrange
            var contentService = new ContentService(Mock.Of<ILogger<ContentService>>(), TestAppSettings.CMS.InvalidURI);

            //Act + Assert
            var exception = await Should.ThrowAsync<Exception>(() => contentService.GetAllItemsAsync());
            exception.InnerException.ShouldBeOfType<WebException>();
        }
    }
}
