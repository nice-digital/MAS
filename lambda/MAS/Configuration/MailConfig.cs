namespace MAS.Configuration
{
    public class MailConfig
    {
        public string ApiKey { get; set; }
        public string ListId { get; set; }
        public int DailyTemplateId { get; set; }
        public string CampaignFolderId { get; set; }
        public string Sender { get; set; }
        public string FromName { get; set; }
    }
}
