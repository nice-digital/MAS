using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    public class Source
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string Id { get; set; }
        [Required, JsonRequired]
        public string Title { get; set; }
    }
}
