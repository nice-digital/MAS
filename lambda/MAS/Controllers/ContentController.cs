using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3.Model;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
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
        public async Task<PutObjectResponse> PutAsync([FromBody] Item item)
        {
            var view = await this.RenderViewAsync("~/Views/ContentView.cshtml", item, false); //TODO needs to move into S3Service
            var response = await _s3Service.WriteToS3(item, view);
            
            return response;
        }
    }
}