using Newtonsoft.Json;
using System.Collections.Generic;
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
        public Source Source { get; set; }
        [JsonProperty("specialities")]
        public List<Speciality> Specialities { get; set; }
        [Required, JsonRequired]
        public EvidenceType EvidenceType { get; set; }
        public string ShortSummary { get; set; }
        public string Comment { get; set; }
        public string ResourceLinks { get; set; }
        public string URL { get; set; }
    }
}
