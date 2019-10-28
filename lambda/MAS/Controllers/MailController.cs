using System.Threading.Tasks;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IContentService _contentService;

        public MailController(IMailService mailService, IContentService contentService)
        {
            _mailService = mailService;
            _contentService = contentService;
        }

        //PUT api/mail/daily
        [HttpPut("daily")]
        public async Task<IActionResult> PutAsync()
        {
            var item = await _contentService.GetItemsAsync();
            var body = item.Title;
            var subject = "Subject";
            var previewText = "Preview text";

            await _mailService.SendToMailChimpAsync(subject, previewText, body);

            return new JsonResult("posted");
        }
    }
}