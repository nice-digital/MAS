using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ILogger<ContentController> _logger;
        private readonly IAmazonS3 _amazonS3;

        public ContentController(IContentService contentService, ILogger<ContentController> logger, IAmazonS3 amazonS3)
        {
            _contentService = contentService;
            _logger = logger; //TODO: Log response errors
            _amazonS3 = amazonS3;
        }

        //PUT api/content/5dac429284dd4afe5eb8fae6
        [HttpPut("{key}")]
        public async Task<IActionResult> PutAsync(string key)
        {
            var item = await _contentService.GetItemAsync(key);

            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = AppSettings.AWSConfig.BucketName,
                Key = key + ".txt",
                ContentBody = item.Title
            };
            var response = await _amazonS3.PutObjectAsync(request);

            return Validate(response.HttpStatusCode);
        }
    }
}