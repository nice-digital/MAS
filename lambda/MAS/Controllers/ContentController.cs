using System.Threading.Tasks;
using Amazon.S3.Model;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController
    {
        private readonly IContentService _contentService;
        private readonly IS3Service _s3Service;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentService contentService, IS3Service s3Service, ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _s3Service = s3Service;
            _logger = logger; //TODO: Log response errors
        }

        //PUT api/content/5dac429284dd4afe5eb8fae6
        [HttpPut("{key}")]
        public async Task<PutObjectResponse> PutAsync(string key)
        {
            var item = await _contentService.GetItemAsync(key);
            var response = await _s3Service.WriteToS3(item);
            
            return response;
        }
    }
}