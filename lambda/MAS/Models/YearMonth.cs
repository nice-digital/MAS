using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    /// <summary>
    /// Year and month containing records
    /// </summary>
    public class YearMonth
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public SeparatedYearMonth Id { get; set; }
    }

    public class SeparatedYearMonth
    {
        [JsonProperty("year"), Required, JsonRequired]
        public int Year { get; set; }

        [JsonProperty("month"), Required, JsonRequired]
        public int Month { get; set; }
    }
}
