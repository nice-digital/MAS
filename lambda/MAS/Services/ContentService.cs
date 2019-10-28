using MAS.Configuration;
using MAS.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IContentService
    {
        Task<Item> GetItemAsync(string itemId);
        Task<Item> GetItemsAsync();
    }

    public class ContentService : IContentService
    {
        public async Task<Item> GetItemAsync(string itemId)
        {
            using (WebClient client = new WebClient())
            {
                var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI + itemId));
                var json = JsonConvert.DeserializeObject<Item>(jsonStr);
                return json;
            }
        }

        public async Task<Item> GetItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI));
                var json = JsonConvert.DeserializeObject<Item>(jsonStr);
                return json;
            }
        }
    }
}
