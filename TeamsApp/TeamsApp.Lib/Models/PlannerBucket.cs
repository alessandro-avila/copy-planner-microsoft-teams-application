using Newtonsoft.Json;

namespace TeamsAppLib.Models
{
    public class PlannerBucket
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("orderHint")]
        public string OrderHint { get; set; }
        [JsonProperty("planId")]
        public string PlanId { get; set; }
        public PlannerTask[] Tasks { get; set; }
    }
}
