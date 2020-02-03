using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IMailService
    {
        Task<Campaign> CreateAndSendDailyAsync(DateTime date, string previewText, string body, List<string> specialitiesInEmail, IEnumerable<Interest> allSpecialities, string receiveEverythingGroupId);
        Task<Campaign> CreateAndSendWeeklyAsync(string previewText, string body, string date);
    }

    public class MailService: IMailService
    {
        #region Constructor

        private readonly IMailChimpManager _mailChimpManager;
        private readonly ILogger<MailService> _logger;
        private readonly MailChimpConfig _mailChimpConfig;
        private readonly MailConfig _mailConfig;

        public MailService(IMailChimpManager mailChimpManager, ILogger<MailService> logger, MailChimpConfig mailChimpConfig, MailConfig mailConfig)
        {
            _mailChimpManager = mailChimpManager;
            _logger = logger;
            _mailChimpConfig = mailChimpConfig;
            _mailConfig = mailConfig;
        }

        #endregion

        public async Task<Campaign> CreateAndSendDailyAsync(DateTime date,
            string previewText,
            string body,
            List<string> specialitiesInEmail,
            IEnumerable<Interest> allSpecialities,
            string receiveEverythingGroupId)
        {
            // Every speciality we receive should exist in the complete list from MailChimp
            var interestIds = specialitiesInEmail.Select(title => allSpecialities.Single(s => s.Name == title).Id).Distinct();

            var dateStr = date.ToString("dd MMMM yyyy");

            try
            {
                var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
                {
                    Type = CampaignType.Regular,
                    Settings = new Setting
                    {
                        FolderId = _mailChimpConfig.CampaignFolderId,
                        TemplateId = _mailChimpConfig.DailyTemplateId,
                        SubjectLine = string.Format(_mailConfig.DailySubject, dateStr),
                        FromName = _mailConfig.FromName,
                        ReplyTo = _mailConfig.ReplyTo,
                        PreviewText = previewText,
                        // We use our own footer in the email template
                        AutoFooter = false
                    },
                    Recipients = new Recipient
                    {
                        ListId = _mailChimpConfig.ListId,
                        SegmentOptions = new SegmentOptions
                        {
                            Match = Match.Any,
                            Conditions = new Condition[]
                            {
                                new Condition
                                {
                                    Type = ConditionType.Interests,
                                    Operator = Operator.InterestContains,
                                    Field = "interests-" + _mailChimpConfig.SpecialityCategoryId,
                                    Value = interestIds.ToArray()
                                },
                                new Condition
                                {
                                    Type = ConditionType.Interests,
                                    Operator = Operator.InterestContains,
                                    Field = "interests-" + _mailChimpConfig.ReceiveEverythingCategoryId,
                                    Value = new string[] { receiveEverythingGroupId }
                                },
                            }
                        }
                    }
                });
                _logger.LogInformation($"Daily campaign has been created with campaignId {campaign.Id}");

                await _mailChimpManager.Content.AddOrUpdateAsync(campaign.Id, new ContentRequest
                {
                    Template = new ContentTemplate
                    {
                        Id = _mailChimpConfig.DailyTemplateId,
                        Sections = new Dictionary<string, object>
                        {
                            { "body", body },
                            // We can't use the mailchimp date merge tag (e.g. *|DATE:jS F|*) because that defaults to today,
                            // and we need to be able to send emails for previous dates in case of errors, so we use a template section instead
                            { "date", dateStr }
                        }
                    }
                });
                _logger.LogInformation($"The body of daily template {_mailChimpConfig.DailyTemplateId} has been updated");

                await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

                return campaign;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send daily email for {dateStr}: {e.Message}");
                throw new Exception($"Failed to send daily email for {dateStr}: {e.Message}", e);
            }  
        }
          
        public async Task<Campaign> CreateAndSendWeeklyAsync(string previewText, string body, string title)
        {
            try
            { 
                var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
                {
                    Type = CampaignType.Regular,
                    Settings = new Setting
                    {
                        FolderId = _mailChimpConfig.CampaignFolderId,
                        TemplateId = _mailChimpConfig.WeeklyTemplateId,
                        SubjectLine = string.Format(_mailConfig.WeeklySubject, title),
                        FromName = _mailConfig.FromName,
                        ReplyTo = _mailConfig.ReplyTo,
                        PreviewText = previewText,
                        AutoFooter = false
                    },
                    Recipients = new Recipient
                    {
                        ListId = _mailChimpConfig.ListId,
                        SegmentOptions = new SegmentOptions
                        {
                            SavedSegmentId = _mailChimpConfig.WeeklySegmentId,
                        }
                    }
                });
                _logger.LogInformation($"Weekly campaign has been created with campaignId {campaign.Id}");
                await _mailChimpManager.Content.AddOrUpdateAsync(campaign.Id, new ContentRequest
                {
                    Template = new ContentTemplate
                    {
                        Id = _mailChimpConfig.WeeklyTemplateId,
                        Sections = new Dictionary<string, object> {
                            { "body", body },
                            { "date", title }
                        }
                    }
                });
                _logger.LogInformation($"The body of weekly template {_mailChimpConfig.WeeklyTemplateId} has been updated");

                await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

                return campaign;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send weekly email for {title}: {e.Message}");
                throw new Exception($"Failed to send weekly email for {title}: {e.Message}, e");
            }
        }
    }
}
