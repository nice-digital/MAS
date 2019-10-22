
using MAS.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Tests.Infrastructure
{
    public class TestAppSettings
    {
        public static AWSConfig GetAWSConfig()
        {
            return new AWSConfig()
            {
                AccessKey = "AKIAIOSFODNN7EXAMPLE",
                SecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
                ServiceURL = "http://localhost:9000",
                BucketName = "demo-bucket"
            };
        }

        public static CMSConfig GetCMSConfig()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/single-item.json").ToString()
            };
        }
    }
}
