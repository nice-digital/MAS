using System;
using System.Threading.Tasks;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IContentService _contentService;
        private readonly ILogger<MailController> _logger;
        private readonly IBankHolidayService _bankHolidayService;

        public MailController(IMailService mailService, IContentService contentService, IBankHolidayService bankHolidayService, ILogger<MailController> logger)
        {
            _mailService = mailService;
            _contentService = contentService;
            _logger = logger;
            _bankHolidayService = bankHolidayService;
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
                var campaignId = await _mailService.CreateAndSendDailyCampaignAsync(subject, previewText, body);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create and send daily campaign - exception: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }

        //PUT api/mail/weekly or api/mail/weekly/2020-01-13
        [HttpPut("weekly/{date?}")]
        public async Task<IActionResult> PutWeeklyMailAsync([FromRoute] DateTime? date = null)
        {
            Weekly weeklyContent;

            var todaysDate = date ?? DateTime.Today;
            var isBankHoliday = _bankHolidayService.IsBankHoliday(todaysDate);
            if (todaysDate.DayOfWeek == DayOfWeek.Monday)
            {
                if (isBankHoliday)
                {
                    _logger.LogWarning($"{todaysDate.ToString("dd/MM/yyyy")} is a bank holiday therefore an email isnt sent");
                    return Content($"{todaysDate.ToString("dd/MM/yyyy")} is a bank holiday therefore an email isnt sent");
                }
                else
                {
                    weeklyContent = await _contentService.GetWeeklyAsync(todaysDate);
                }
            }
            else
            {
                var previousMonday = todaysDate.AddDays(-(int)todaysDate.DayOfWeek + 1);
                var previousMondayIsBankHoliday = _bankHolidayService.IsBankHoliday(previousMonday);
                if (previousMondayIsBankHoliday)
                {
                    weeklyContent = await _contentService.GetWeeklyAsync(previousMonday);
                }
                else
                {
                    _logger.LogWarning($"An email was sent on {previousMonday.ToString("dd/MM/yyyy")}");
                    return Content($"An email was sent on {previousMonday.ToString("dd/MM/yyyy")}");
                }
            }

            if (weeklyContent == null)
            {
                _logger.LogWarning("No weekly record was found");
                return Content("No weekly record was found");
            }
            else if (weeklyContent.Items.Count == 0)
            {
                _logger.LogWarning("The weekly didn't have any items");
                return Content("The weekly didn't have any items");
            }

            var body = _mailService.CreateWeeklyEmailBody(weeklyContent);
            var subject = "NICE Medicines Awareness Weekly - " + weeklyContent.Title;
            var previewText = "NICE Medicines Awareness Weekly. A selection of the week's current awareness and evidence-based medicines information";

            try
            {
                var campaignId = await _mailService.CreateAndSendWeeklyCampaignAsync(subject, previewText, body);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create and send weekly campaign - exception: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}