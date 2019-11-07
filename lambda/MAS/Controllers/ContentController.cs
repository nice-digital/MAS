using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly IS3Service _s3Service;
        private readonly ILogger<ContentController> _logger;
        private readonly IAmazonS3 _amazonS3;

        public ContentController(IContentService contentService, IS3Service s3Service, ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _s3Service = s3Service;
            _logger = logger; //TODO: Log response errors
        }

        //PUT api/content/5dac429284dd4afe5eb8fae6
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] string json)
        {
            var item = JsonConvert.DeserializeObject<Item>(json);
            var response = _s3Service.WriteToS3(item);

            return Validate(response.Result.HttpStatusCode);
        }
    }
}