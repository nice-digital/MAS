using System;
using System.Threading.Tasks;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IS3Service s3Service, ILogger<ContentController> logger)
        {
            _s3Service = s3Service;
            _logger = logger;
        }

        //PUT api/content/
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Item item)
        {
            try
            {
                var response = await _s3Service.WriteToS3(item);
                return Validate(response.HttpStatusCode, _logger);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to write an item to the S3 bucket - exception: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}