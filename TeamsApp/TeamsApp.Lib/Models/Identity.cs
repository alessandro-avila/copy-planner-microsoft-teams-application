using Newtonsoft.Json;
using System;

namespace TeamsAppLib.Models
{
    public class Identity
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string TenantId { get; set; }
    }
}
