using System;
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
        public async Task<string> PutMailAsync()
        {
            var items = await _contentService.GetItemsAsync();

            var body = "";
            foreach (var item in items)
            {
                body += item.Title + "<br><br><br>";
            }

            var subject = "MAS Email";
            var previewText = "This MAS email was created " + DateTime.Now.ToShortDateString();

            var campaignId = await _mailService.CreateAndSendCampaignAsync(subject, previewText, body);

            return campaignId;
        }
    }
}