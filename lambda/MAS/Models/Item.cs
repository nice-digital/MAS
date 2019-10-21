using Newtonsoft.Json;

namespace MAS.Models
{
    public class Item
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
