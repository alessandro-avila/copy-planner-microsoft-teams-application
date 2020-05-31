using Newtonsoft.Json;
using System;

namespace TeamsAppLib.Models
{
    public class PlannerPlan
    {
        [JsonProperty("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("createdBy")]
        public IdentitySet CreatedBy { get; set; }
        [JsonProperty("contexts")]
        public object Contexts { get; set; }
        public PlannerBucket Buckets { get; set; }
        public PlannerTask Tasks { get; set; }
    }
}
