using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    /// <summary>
    /// A 'lighter' version of the Item, returned from the all items feed.
    /// </summary>
    public class ItemLight
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string Id { get; set; }

        [JsonProperty("slug"), Required, JsonRequired]
        public string Slug { get; set; }

        [JsonProperty("title"), Required, JsonRequired]
        public string Title { get; set; }

        [JsonProperty("updatedAt"), Required, JsonRequired]
        public DateTime UpdatedAt { get; set; }
    }
}
