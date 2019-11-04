using MAS.Configuration;
using MAS.Controllers;
using MAS.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IContentService
    {
        Task<Item> GetItemAsync(string itemId);
    }

    public class ContentService : IContentService
    {
        private readonly ILogger<ContentController> _logger;
        public ContentService(ILogger<ContentController> logger)
        {
            _logger = logger;
        }
        public async Task<Item> GetItemAsync(string itemId)
        {
            using (WebClient client = new WebClient())
            {
                var json = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI + itemId));
                Item item = null;
                try
                {
                    item = JsonConvert.DeserializeObject<Item>(json);
                }
                catch(Exception e)
                {
                    _logger.Log(LogLevel.Error, e.Message);
                    throw e;
                }
                
                return item;
            }
        }
    }
}
