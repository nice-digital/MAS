using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IContentService
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<Weekly> GetWeeklyAsync(DateTime sendDate);
    }

    public class ContentService : IContentService
    {
        private readonly ILogger<ContentService> _logger;

        public ContentService(ILogger<ContentService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI));
                    var json = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                    return json;
                }
                catch(Exception e)
                {
                    _logger.LogError($"Failed to get items from CMS - exception: {e.Message}");
                    throw new Exception($"Failed to get items from CMS - exception: {e.Message}");
                }
            }
        }

        public async Task<Weekly> GetWeeklyAsync(DateTime sendDate)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI + AppSettings.CMSConfig.WeekliesBySendDate + sendDate));
                    var json = JsonConvert.DeserializeObject<Weekly>(jsonStr);
                    return json;
                }
                catch(Exception e)
                {
                    _logger.LogError($"Failed to get weekly item from CMS - exception: {e.Message}");
                    throw new Exception($"Failed to get weekly item from CMS - exception: {e.Message}");
                }
            }
        }
    }
}
