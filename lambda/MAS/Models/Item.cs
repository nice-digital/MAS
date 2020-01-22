using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    public class Item : ItemLight
    {
        [JsonProperty("source"), Required, JsonRequired]
        public Source Source { get; set; }

        [JsonProperty("specialities")]
        public List<Speciality> Specialities { get; set; }

        [JsonProperty("evidenceType"), Required, JsonRequired]
        public EvidenceType EvidenceType { get; set; }

        [JsonProperty("shortSummary")]
        public string ShortSummary { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("commentUrl")]
        public string CommentUrl { get; set; }

        [JsonProperty("resourceLinks")]
        public string ResourceLinks { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("publicationDate")]
        public DateTime? PublicationDate { get; set; }

        [JsonProperty("createdAt"), Required, JsonRequired]
        public DateTime CreatedAt { get; set; }
    }
}
