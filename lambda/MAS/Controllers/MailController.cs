using System;
using System.Threading.Tasks;
using MAS.Configuration;
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
        public async Task<IActionResult> PutMailAsync()
        {
            var items = await _contentService.GetItemsAsync();

            var body = _mailService.CreateDailyEmailBody(items);
            var subject = "MAS Email";
            var previewText = "This MAS email was created " + DateTime.Now.ToShortDateString();

            try
            {
                var campaignId = await _mailService.CreateAndSendCampaignAsync(subject, previewText, body, AppSettings.MailConfig.DailyTemplateId);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }

        //PUT api/mail/weekly
        [HttpPut("weekly")]
        public async Task<IActionResult> PutWeeklyMailAsync()
        {
            var weekly = await _contentService.GetWeeklyAsync();

            var body = _mailService.CreateWeeklyEmailBody(weekly);
            var subject = "MAS Weekly Email";
            var previewText = "This MAS email was created " + DateTime.Now.ToShortDateString();

            try
            {
                var campaignId = await _mailService.CreateAndSendCampaignAsync(subject, previewText, body, AppSettings.MailConfig.WeeklyTemplateId);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}