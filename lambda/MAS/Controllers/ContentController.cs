using System.Threading.Tasks;
using Amazon.S3.Model;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
    {
        private readonly IS3Service _s3Service;
        private readonly IStaticContentWriter _staticContentWriter;
        private readonly ILogger<ContentController> _logger;
        
        public ContentController(IS3Service s3Service, IStaticContentWriter staticContentWriter, ILogger<ContentController> logger)
        {
            _s3Service = s3Service;
            _staticContentWriter = staticContentWriter;
            _logger = logger;
        }

        //PUT api/content/
        [HttpPut]
        public async Task<PutObjectResponse> PutAsync([FromBody] Item item)
        {
            var body = await _staticContentWriter.RenderViewAsync(this, "~/Views/ContentView.cshtml", item, false);
            var response = await _s3Service.WriteToS3(item, body);
            
            return response;
        }
    }
}