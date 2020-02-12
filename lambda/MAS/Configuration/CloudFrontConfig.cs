using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Configuration
{
    public class CloudFrontConfig
    {
        public static CloudFrontConfig Current { get; private set; }

        public CloudFrontConfig()
        {
            Current = this;
        }

        public bool Enabled { get; set; }
        public string DistributionID { get; set; }
    }
}
