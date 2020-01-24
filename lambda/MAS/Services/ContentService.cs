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
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime? date = null);
        Task<Weekly> GetWeeklyAsync(DateTime sendDate);
    }

    public class ContentService : IContentService
    {
        #region Constructor

        private readonly ILogger<ContentService> _logger;
        private readonly CMSConfig _cmsConfig;

        public ContentService(ILogger<ContentService> logger, CMSConfig cmsConfig)
        {
            _logger = logger;
            _cmsConfig = cmsConfig;
        }

        #endregion

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(_cmsConfig.BaseUrl + _cmsConfig.AllItemsPath));
                    var items = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                    return items;
                }
                catch(Exception e)
                {
                    _logger.LogError(e, $"Failed to get all items from CMS");
                    throw new Exception($"Failed to get all items from CMS", e);
                }
            }
        }

        public async Task<Weekly> GetWeeklyAsync(DateTime sendDate)
        {
            using (WebClient client = new WebClient())
            {
                var path = string.Format(_cmsConfig.WeekliesBySendDate, sendDate.ToString("yyyy-MM-dd"));
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(_cmsConfig.BaseUrl + path));
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

        public async Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime? date = null)
        {
            date = date ?? DateTime.Today;

            using (WebClient client = new WebClient())
            {
                var path = string.Format(_cmsConfig.DailyItemsPath, date.Value.ToString("yyyy-MM-dd"));
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(_cmsConfig.BaseUrl + path));
                    var items = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                    return items;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to get daily items from CMS");
                    throw new Exception($"Failed to get daily items from CMS", e);
                }
            }
        }

    }
}
