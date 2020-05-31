using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamsAppLib.Models
{
    public class PlannerPlanDetails
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }
        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sharedWith")]
        public Dictionary<string, bool> SharedWith { get; set; }
        [JsonProperty("categoryDescriptions")]
        public CategoryDescriptions CategoryDescriptions { get; set; }
        [JsonProperty("contextDetails")]
        public object ContextDetails { get; set; }
    }

    public class CategoryDescriptions
    {
        [JsonProperty("category1")]
        public object Category1 { get; set; }
        [JsonProperty("category2")]
        public object Category2 { get; set; }
        [JsonProperty("category3")]
        public object Category3 { get; set; }
        [JsonProperty("category4")]
        public object Category4 { get; set; }
        [JsonProperty("category5")]
        public object Category5 { get; set; }
        [JsonProperty("category6")]
        public object Category6 { get; set; }
    }
}
