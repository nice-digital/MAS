using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IMailService
    {
        Task<string> CreateAndSendCampaignAsync(string subject, string previewText, string body);
        string CreateEmailBody(Item item);
    }

    public class MailService: IMailService
    {
        private readonly IMailChimpManager _mailChimpManager;

        public MailService(IMailChimpManager mailChimpManager)
        {
            _mailChimpManager = mailChimpManager;
        }

        public async Task<string> CreateAndSendCampaignAsync(string subject, string previewText, string body)
        {
            var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
            {
                Type = CampaignType.Regular,
                Settings = new Setting
                {
                    FolderId = AppSettings.MailConfig.CampaignFolderId,
                    TemplateId = AppSettings.MailConfig.DailyTemplateId,
                    SubjectLine = subject,
                    FromName = "MAS",
                    ReplyTo = "MAS@nice.org.uk",
                    PreviewText = previewText
                },
                Recipients = new Recipient
                {
                    ListId = AppSettings.MailConfig.ListId
                }
            });
            
            await _mailChimpManager.Content.AddOrUpdateAsync(campaign.Id, new ContentRequest
            {
                Template = new ContentTemplate
                {
                    Id = AppSettings.MailConfig.DailyTemplateId,
                    Sections = new Dictionary<string, object> {
                        { "body", body }
                    }
                }
            });

            await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

            return campaign.Id;
        }

        public string CreateEmailBody(Item item)
        {
            var emailBody = new StringBuilder();
            emailBody.Append(item.Title);
            emailBody.Append("<br>");
            emailBody.Append(item.ShortSummary);
            emailBody.Append("<br>");
            emailBody.Append(item.Source.Title);
            emailBody.Append("<br><br><br>");
            return emailBody.ToString();
        }
    }
}
