using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace MAS.Models
{
    public class GreatBritain
    {
        [JsonProperty("england-and-wales")]
        public Country EnglandAndWales { get; set; }
    }
}
