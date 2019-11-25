using MAS.Configuration;
using MAS.Models;
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
    }

    public class ContentService : IContentService
    {
        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI));
                var json = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                return json;
            }
        }
    }
}
