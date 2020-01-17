﻿using MAS.Configuration;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IContentService _contentService;
        private readonly IViewRenderer _viewRenderer;
        private readonly ILogger<MailService> _logger;

        public MailController(IMailService mailService, IContentService contentService, IViewRenderer viewRenderer, ILogger<MailService> logger)
        {
            _mailService = mailService;
            _contentService = contentService;
            _viewRenderer = viewRenderer;
            _logger = logger;
        }

        //PUT api/mail/daily?date=01-01-2020
        [HttpPut("daily")]
        public async Task<IActionResult> PutMailAsync(DateTime? date = null)
        {
            var items = await _contentService.GetDailyItemsAsync(date);
            var body = await _viewRenderer.RenderViewAsync(this, "~/Views/DailyEmail.cshtml", items.ToList());
            var previewText = "The very latest current awareness and evidence-based medicines information";

            var env = AppSettings.EnvironmentConfig.Name;
            env = env == "Live" ? "" : env + ": ";
            var subject = String.Format("{0}NICE Medicines Awareness Daily {1}", env, ((DateTime)date).ToString("dd MMMM yyyy"));

            

            try
            {
                var campaignId = await _mailService.CreateAndSendDailyAsync(subject, previewText, body);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send daily email - exception: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}