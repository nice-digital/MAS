using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    public class Item
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string Id { get; set; }
        [Required, JsonRequired]
        public string Title { get; set; }
        [Required, JsonRequired]
        public string Source { get; set; }
        [Required, JsonRequired]
        public string ShortSummary { get; set; }
        [Required, JsonRequired]
        public string EvidenceType { get; set; }
        [Required, JsonRequired]
        public string UKMiComment { get; set; }
        [Required, JsonRequired]
        public string ResourceLinks { get; set; }
    }
}
