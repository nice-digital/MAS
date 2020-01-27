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
        Task<IEnumerable<ItemLight>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime date);
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

        public async Task<IEnumerable<ItemLight>> GetAllItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(_cmsConfig.BaseUrl + _cmsConfig.AllItemsPath));
                    var items = JsonConvert.DeserializeObject<ItemLight[]>(jsonStr);
                    return items;
                }
                catch(Exception e)
                {
                    _logger.LogError(e, $"Failed to get all items from CMS");
                    throw new Exception($"Failed to get all items from CMS", e);
                }
            }
        }

        public async Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime date)
        {
            using (WebClient client = new WebClient())
            {
                var path = string.Format(_cmsConfig.DailyItemsPath, date.ToString("yyyy-MM-dd"));
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
