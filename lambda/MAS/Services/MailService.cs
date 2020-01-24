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
        Task<Campaign> CreateAndSendDailyAsync(string subject, string previewText, string body, List<string> specialitiesInEmail, IEnumerable<Interest> allSpecialities, string receiveEverythingGroupId);
        Task<string> CreateAndSendWeeklyAsync(string subject, string previewText, string body, string date);
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

        public async Task<Campaign> CreateAndSendDailyAsync(string subject,
            string previewText,
            string body,
            List<string> specialitiesInEmail,
            IEnumerable<Interest> allSpecialities,
            string receiveEverythingGroupId)
        {
            // Every speciality we receive should exist in the complete list from MailChimp
            var interestIds = specialitiesInEmail.Select(title => allSpecialities.Single(s => s.Name == title).Id).Distinct();

            try
            {
                var campaign = await _mailChimpManager.Campaigns.AddAsync(new Campaign
                {
                    Type = CampaignType.Regular,
                    Settings = new Setting
                    {
                        FolderId = _mailChimpConfig.CampaignFolderId,
                        TemplateId = _mailChimpConfig.DailyTemplateId,
                        SubjectLine = subject,
                        FromName = _mailConfig.FromName,
                        ReplyTo = _mailConfig.ReplyTo,
                        PreviewText = previewText
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
                            { "body", body }
                        }
                    }
                });
                _logger.LogInformation($"The body of daily template {_mailChimpConfig.DailyTemplateId} has been updated");

                await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

                return campaign;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to communicate with MailChimp - exception: {e.Message}");
                throw new Exception($"Failed to communicate with MailChimp - exception: {e.Message}");
            }  
        }
          
        public async Task<string> CreateAndSendWeeklyAsync(string subject, string previewText, string body, string date)
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
                        SubjectLine = subject,
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
                            { "date", date }
                        }
                    }
                });
                _logger.LogInformation($"The body of weekly template {_mailChimpConfig.DailyTemplateId} has been updated");

                await _mailChimpManager.Campaigns.SendAsync(campaign.Id.ToString());

                return campaign.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send weekly email for {date}: {e.Message}");
                throw new Exception($"Failed to send weekly email for {date}: {e.Message}, e");
            }
        }
    }
}
