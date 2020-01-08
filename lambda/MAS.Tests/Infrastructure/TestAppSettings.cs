using MAS.Configuration;
using System;
using System.IO;

namespace MAS.Tests.Infrastructure
{
    public class TestAppSettings
    {
        public static CMSConfig GetInvalidURI()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/nonexistanturl").ToString()
            };
        }

        public static CMSConfig GetMultipleItemsFeed()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/multiple-items.json").ToString()
            };
        }

        public static CMSConfig GetWeeklyFeed()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/weekly.json").ToString()
            };
        }
    }
}
