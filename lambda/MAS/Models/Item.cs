using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MAS.Models
{
    public class Item
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; }
        public string Source { get; set; } //TODO: Needs source item values instead of source item Ids
        public string GeographicalCoverage { get; set; }
        public IEnumerable<string> Speciality { get; set; } //TODO: Needs speciality item values instead of Ids
        public string ShortSummary { get; set; }
        public string ResourceLinks { get; set; }
        public string UKMiComment { get; set; }
        public int MAWScore { get; set; }
    }
}
