using System.Threading.Tasks;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly IS3Service _s3Service;

        public ContentController(IContentService contentService, IS3Service s3Service)
        {
            _contentService = contentService;
            _s3Service = s3Service;
        }

        //PUT api/content/5dac429284dd4afe5eb8fae6
        [HttpPut("{key}")]
        public async Task<IActionResult> PutAsync(string key)
        {
            var item = await _contentService.GetItemAsync(key);
            var response = await _s3Service.WriteToS3(item);
            
            return Validate(response.HttpStatusCode);
        }
    }
}