using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IMailService
    {
        Task<string> CreateAndSendDailyAsync(string subject, string previewText, string body);
    }

    public class MailService: IMailService
    {
        private readonly IMailChimpManager _mailChimpManager;
        private readonly ILogger<MailService> _logger;

        public MailService(IMailChimpManager mailChimpManager, ILogger<MailService> logger)
        {
            _mailChimpManager = mailChimpManager;
            _logger = logger;
        }

        public async Task<string> CreateAndSendDailyAsync(string subject, string previewText, string body)
        {
            try
            {
                var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
                {
                    Type = CampaignType.Regular,
                    Settings = new Setting
                    {
                        FolderId = AppSettings.MailChimpConfig.CampaignFolderId,
                        TemplateId = AppSettings.MailChimpConfig.DailyTemplateId,
                        SubjectLine = subject,
                        FromName = AppSettings.MailConfig.FromName,
                        ReplyTo = AppSettings.MailConfig.ReplyTo,
                        PreviewText = previewText
                    },
                    Recipients = new Recipient
                    {
                        ListId = AppSettings.MailChimpConfig.ListId
                    }
                });

                await _mailChimpManager.Content.AddOrUpdateAsync(campaign.Id, new ContentRequest
                {
                    Template = new ContentTemplate
                    {
                        Id = AppSettings.MailChimpConfig.DailyTemplateId,
                        Sections = new Dictionary<string, object> {
                        { "body", body }
                    }
                    }
                });

                await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

                return campaign.Id;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to communitcate with MailChimp - exception: {e.Message}");
                throw new Exception($"Failed to communitcate with MailChimp - exception: {e.Message}");
            }
          
        }
    }
}
