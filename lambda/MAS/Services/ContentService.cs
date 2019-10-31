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
                var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI + itemId));
                Item json = null;
                try
                {
                    json = JsonConvert.DeserializeObject<Item>(jsonStr);
                }
                catch(Exception e)
                {
                    throw e;
                }
                
                return json;
            }
        }
    }
}
