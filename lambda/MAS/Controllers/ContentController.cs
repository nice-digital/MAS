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
        private readonly IS3Service _s3Service;
        private readonly ILogger<ContentController> _logger;
        private readonly IAmazonS3 _amazonS3;

        public ContentController(IS3Service s3Service, ILogger<ContentController> logger)
        {
            _s3Service = s3Service;
            _logger = logger; //TODO: Log response errors
        }

        //PUT api/content/
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Item item)
        {
            var response = _s3Service.WriteToS3(item);

            return Validate(response.Result.HttpStatusCode);
        }
    }
}