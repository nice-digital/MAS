namespace MAS.Configuration
{
    public class AWSConfig
    {
        public static AWSConfig Current { get; private set; }

        public AWSConfig()
        {
            Current = this;
        }

        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ServiceURL { get; set; }
        public string BucketName { get; set; }
        public string StaticURL { get; set; }
        //This should bind correctly despit it being a nested object in the application.json
        public string DistributionId { get; set; }
    }
}
