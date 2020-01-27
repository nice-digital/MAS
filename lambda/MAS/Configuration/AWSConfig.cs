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
        public string DistributionId { get; set; }
    }
}
