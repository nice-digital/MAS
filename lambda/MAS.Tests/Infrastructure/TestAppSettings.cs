using MAS.Configuration;
using System;
using System.IO;

namespace MAS.Tests.Infrastructure
{
    public class TestAppSettings
    { 
        public static CMSConfig GetSingleItemFeed()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/single-item.json").ToString()
            };
        }

        public static CMSConfig GetMultipleItemsFeed()
        {
            return new CMSConfig()
            {
                URI = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/multiple-items.json").ToString()
            };
        }
    }
}
