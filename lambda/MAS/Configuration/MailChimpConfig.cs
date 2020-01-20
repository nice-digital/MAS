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
    }
}
