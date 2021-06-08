using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    /// <summary>
    /// Year and month containing records
    /// </summary>
    public class YearMonth
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string YearMonthDate { get; set; }
    }
}
