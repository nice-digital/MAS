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
        //[Required, JsonRequired]
        //public DateTime PublicationDate { get; set; }
        //[Required, JsonRequired]
        //public DateTime CreatedDate { get; set; }
        //[Required, JsonRequired]
        //public string Category { get; set; }
        [Required, JsonRequired]
        public string Source { get; set; } //TODO: Needs source item values instead of source item Ids
        //[Required, JsonRequired]
       // public IEnumerable<string> Speciality { get; set; } //TODO: Needs speciality item values instead of Ids
        [Required, JsonRequired]
        public string ShortSummary { get; set; }
        //public string ResourceLinks { get; set; }
        //[Required, JsonRequired]
        //public string UKMiComment { get; set; }
        //[Required, JsonRequired]
        //public int RelevancyScore { get; set; }
    }
}
