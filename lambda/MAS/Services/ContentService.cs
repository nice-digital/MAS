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
    }

    public class ContentService : IContentService
    {
        public async Task<Item> GetItemAsync(string itemId)
        {
            using (WebClient client = new WebClient())
            {
                string jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI + itemId));

                Item json = JsonConvert.DeserializeObject<Item>(jsonStr);

                return json;
            }
        }
    }
}
