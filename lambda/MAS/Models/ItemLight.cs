using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Models
{
    /// <summary>
    /// A
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
