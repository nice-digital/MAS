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
        private readonly IStaticContentService _staticContentService;
        private readonly IViewRenderer _viewRenderer;
        private readonly ILogger<ContentController> _logger;
        
        public ContentController(IStaticContentService contentService, IViewRenderer viewRenderer, ILogger<ContentController> logger)
        {
            _staticContentService = contentService;
            _viewRenderer = viewRenderer;
            _logger = logger;
        }

        //PUT api/content/
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Item item)
        {
            try
            {
            var body = await _viewRenderer.RenderViewAsync(this, "~/Views/ContentView.cshtml", item, false);
            var response = await _staticContentService.Write(item.Slug, body);
            return Validate(response.HttpStatusCode, _logger);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}