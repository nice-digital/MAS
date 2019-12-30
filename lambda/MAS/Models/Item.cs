﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    public class Item
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string Id { get; set; }
        [Required, JsonRequired]
        public string Slug { get; set; }
        [Required, JsonRequired]
        public string Title { get; set; }
        [Required, JsonRequired]
        public string URL { get; set; }
        [Required, JsonRequired]
        public Source Source { get; set; }
        [Required, JsonRequired]
        public string ShortSummary { get; set; }
        public EvidenceType EvidenceType { get; set; }
        public string UKMiComment { get; set; }
        public string ResourceLinks { get; set; }
    }
}
