using MAS.Configuration;
using System;
using System.IO;

namespace MAS.Tests.Infrastructure
{
    /// <summary>
    /// Use this in integration tests to override CMS config per test e.g. AppSettings.CMSConfig = TestAppSettings.CMS.InvalidURI;
    /// </summary>
    public static class TestAppSettings
    {
        public static class BankHoliday
        {
            public static BankHolidayConfig Default
            {
                get
                {
                    return new BankHolidayConfig()
                    {
                        SourceURL = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/").ToString() + "bank-holidays.json",
                    };

                }
            }
        }

        public static class CMS
        {
            public static CMSConfig Default
            {
                get
                {
                    return new CMSConfig()
                    {
                        BaseUrl = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/").ToString(),
                        AllItemsPath = "all-items.json",
                        DailyItemsPath = "daily-items.json",
                        WeekliesBySendDatePath = "weekly.json",
                        MonthsItemsPath = "month-items.json",
                        YearMonthsPath = "yearMonths.json"
                    };
                }
            }

            public static CMSConfig InvalidURI
            {
                get
                {
                    return new CMSConfig()
                    {
                        BaseUrl = new Uri("file://" + Directory.GetCurrentDirectory() + "/Feeds/nonexistanturl").ToString()
                    };
                }
            }
        }

        public static class MailChimp
        {
            public static MailChimpConfig Default
            {
                get
                {
                    return new MailChimpConfig()
                    {
                        ApiKey = "api-key",
                        CampaignFolderId = "campaign-folder-id",
                        DailyTemplateId = 99,
                        ListId = "list-id",
                        ReceiveEverythingCategoryId = "receive-everything-category-id",
                        SpecialityCategoryId = "speciality-category-id"
                    };
                }
            }
        }
    }
}
