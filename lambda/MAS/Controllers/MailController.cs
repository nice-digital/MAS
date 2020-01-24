using MailChimp.Net.Interfaces;
using MAS.Configuration;
using MAS.Services;
using MAS.ViewModels;
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
        #region Constructor

        private readonly IMailChimpManager _mailChimpManager;
        private readonly IMailService _mailService;
        private readonly IContentService _contentService;
        private readonly IViewRenderer _viewRenderer;
        private readonly ILogger<MailService> _logger;
        private readonly MailConfig _mailConfig;
        private readonly MailChimpConfig _mailChimpConfig;

        public MailController(IMailChimpManager mailChimpManager, IMailService mailService, IContentService contentService, IViewRenderer viewRenderer, ILogger<MailService> logger, MailConfig mailConfig, MailChimpConfig mailChimpConfig)
        {
            _mailChimpManager = mailChimpManager;
            _mailService = mailService;
            _contentService = contentService;
            _viewRenderer = viewRenderer;
            _logger = logger;
            _mailConfig = mailConfig;
            _mailChimpConfig = mailChimpConfig;
        }

        #endregion

        //PUT api/mail/daily?date=01-01-2020
        [HttpPut("daily")]
        public async Task<IActionResult> PutMailAsync(DateTime? date = null)
        {
            var sendDate = date ?? DateTime.Today;

            // Load all the required data in parallel
            var itemsTask = _contentService.GetDailyItemsAsync(sendDate);
            var specialitiesCategoryTask = _mailChimpManager.InterestCategories.GetAsync(_mailChimpConfig.ListId, _mailChimpConfig.SpecialityCategoryId);
            var specialitiesGroupsTask = _mailChimpManager.Interests.GetAllAsync(_mailChimpConfig.ListId, _mailChimpConfig.SpecialityCategoryId);
            var receiveEverythingCategoryTask = _mailChimpManager.InterestCategories.GetAsync(_mailChimpConfig.ListId, _mailChimpConfig.ReceiveEverythingCategoryId);
            var receiveEverythingGroupsTask = _mailChimpManager.Interests.GetAllAsync(_mailChimpConfig.ListId, _mailChimpConfig.ReceiveEverythingCategoryId);

            // Wait for all the data to be loaded
            var items = await itemsTask;
            var specialitiesCategory = await specialitiesCategoryTask;
            var specialitiesGroups = await specialitiesGroupsTask;
            var receiveEverythingCategory = await receiveEverythingCategoryTask;
            var receiveEverythingGroups = await receiveEverythingGroupsTask;

            // Validate all the data is present and correct
            if (!items.Any())
            {
                var message = $"Not sending email for {sendDate}, no items returned from the CMS";
                _logger.LogWarning(message);
                return Content(message);
            }
            else if (specialitiesCategory == null)
            {
                var message = $"Couldn't find specialities group category with id {_mailChimpConfig.SpecialityCategoryId}";
                _logger.LogError(message);
                return StatusCode(500, message);
            }
            else if (receiveEverythingCategory == null)
            {
                var message = $"Couldn't find 'send me everything' category with id {_mailChimpConfig.ReceiveEverythingCategoryId}";
                _logger.LogError(message);
                return StatusCode(500, message);
            }
            else if (receiveEverythingGroups.Count() != 1)
            {
                var message = $"There should only be 1 group within the 'send me everything' group category but found {receiveEverythingGroups.Count()}";
                _logger.LogError(message);
                return StatusCode(500, message);
            }

            var viewModel = new DailyEmailViewModel {
                Items = items.ToList(),
                SpecialitiesGroupCategoryName = specialitiesCategory.Title,
                EverythingGroupCategoryName = receiveEverythingCategory.Title,
                EverythingGroupName = receiveEverythingGroups.Single().Name
            };

            var body = await _viewRenderer.RenderViewAsync(this, "~/Views/Mail/Daily.cshtml", viewModel);
            var previewText = "The very latest current awareness and evidence-based medicines information";


            try
            {
                var campaign = await _mailService.CreateAndSendDailyAsync(
                    sendDate, 
                    previewText, 
                    body, 
                    items.SelectMany(x => x.Specialities).Select(y => y.Title).ToList(),
                    specialitiesGroups,
                    receiveEverythingGroups.Single().Id);
                return Json(campaign);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send daily email - exception: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}