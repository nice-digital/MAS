namespace MAS.Configuration
{
    public class MailChimpConfig
    {
        public static MailChimpConfig Current { get; private set; }

        public MailChimpConfig()
        {
            Current = this;
        }

        public string ApiKey { get; set; }
        public string ListId { get; set; }
        public int DailyTemplateId { get; set; }
        public string CampaignFolderId { get; set; }
        public string SpecialityCategoryId { get; set; }
        public string ReceiveEverythingCategoryId { get; set; }
        public int WeeklyTemplateId { get; set; }
        public int WeeklySegmentId { get; set; }
        public int DailySegmentId { get; set; }
    }
}
