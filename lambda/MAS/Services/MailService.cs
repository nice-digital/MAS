using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using Microsoft.Extensions.Logging;
using MAS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IMailService
    {
        Task<string> CreateAndSendCampaignAsync(string subject, string previewText, string body, int templateId, int segmentId);
        string CreateDailyEmailBody(IEnumerable<Item> item);
        string CreateWeeklyEmailBody(Weekly weekly);
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

        public async Task<string> CreateAndSendCampaignAsync(string subject, string previewText, string body, int templateId, int segmentId)
        {
            try
            {
                var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
                {
                    Type = CampaignType.Regular,
                    Settings = new Setting
                    {
                        FolderId = AppSettings.MailConfig.CampaignFolderId,
                        TemplateId = templateId,
                        SubjectLine = subject,
                        FromName = "MAS",
                        ReplyTo = "MAS@nice.org.uk",
                        PreviewText = previewText
                    },
                    Recipients = new Recipient
                    {
                        ListId = AppSettings.MailConfig.ListId,
                        SegmentOptions = new SegmentOptions {
                            SavedSegmentId = segmentId
                        }
                    }
                });

                await _mailChimpManager.Content.AddOrUpdateAsync(campaign.Id, new ContentRequest
                {
                    Template = new ContentTemplate
                    {
                        Id = templateId,
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

        public string CreateDailyEmailBody(IEnumerable<Item> items)
        {
            var body = new StringBuilder();

            foreach (var item in items)
            {
                body.Append(item.Source.Title);
                body.Append("<br>");
                body.Append(item.Title);
                body.Append("<br>");
                body.Append(item.ShortSummary);
                body.Append("<br><br><br>");
            }

            return body.ToString();
        }

        public string CreateWeeklyEmailBody(Weekly weekly)
        {
            var body = new StringBuilder();

            body.Append(weekly.Title);
            body.Append("<br>");
            body.Append(weekly.CommentaryTitle);
            body.Append("<br>");
            body.Append(weekly.CommentarySummary);
            body.Append("<br><br><br>");

            foreach (var item in weekly.Items)
            {
                body.Append(item.Source.Title);
                body.Append("<br>");
                body.Append(item.Title);
                body.Append("<br>");
                body.Append(item.ShortSummary);
                body.Append("<br><br><br>");
            }

            body.Append(weekly.CommentarySummary);
            body.Append(weekly.CommentaryBody);

            return body.ToString();
        }
    }
}
