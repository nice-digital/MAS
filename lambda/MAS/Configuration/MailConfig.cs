namespace MAS.Configuration
{
    public class MailConfig
    {
        public string ApiKey { get; set; }
        public string ListId { get; set; }
        public int DailyTemplateId { get; set; }
        public string CampaignFolderId { get; set; }
        public int WeeklyTemplateId { get; set; }
        public int WeeklySegmentId { get; set; }
        public int DailySegmentId { get; set; }
    }
}
