using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MAS.Models
{
    /// <summary>
    /// A year and month which contains a record in the CMS eg 2021-06
    /// </summary>
    public class YearMonth
    {
        [JsonProperty("_id"), Required, JsonRequired]
        public string YearMonthDate { get; set; }
    }
}
