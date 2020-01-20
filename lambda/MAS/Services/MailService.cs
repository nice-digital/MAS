using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Configuration;
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
        Task<string> CreateAndSendDailyAsync(string subject, string previewText, string body, List<string> specialitiesInEmail);
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

        public async Task<string> CreateAndSendDailyAsync(string subject, string previewText, string body, List<string> specialitiesInEmail)
        {
            var interests = await _mailChimpManager.Interests.GetAllAsync(_mailChimpConfig.ListId, _mailChimpConfig.SpecialityCategoryId);

            var interestIds = new List<string>();
            foreach (string item in specialitiesInEmail)
            {
                var interest = interests.FirstOrDefault(x => x.Name == item);
                interestIds.Add(interest.Id);
            }


            var receiveEverythingOptionId = _mailChimpManager
                .Interests
                .GetAllAsync(_mailChimpConfig.ListId, _mailChimpConfig.ReceiveEverythingCategoryId)
                .Result
                .First()
                .Id;

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
                                    Value = new string[] { receiveEverythingOptionId }
                                },
                            }
                        }
                    }
                });

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
