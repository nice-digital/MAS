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
        private readonly IStaticContentService _s3Service;
        private readonly IViewRenderer _staticContentWriter;
        private readonly ILogger<ContentController> _logger;
        
        public ContentController(IStaticContentService s3Service, IViewRenderer staticContentWriter, ILogger<ContentController> logger)
        {
            _s3Service = s3Service;
            _staticContentWriter = staticContentWriter;
            _logger = logger;
        }

        //PUT api/content/
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Item item)
        {
            try
            {
            var body = await _staticContentWriter.RenderViewAsync(this, "~/Views/ContentView.cshtml", item, false);
            var response = await _s3Service.WriteToS3(item.Slug, body);
            return Validate(response.HttpStatusCode, _logger);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}