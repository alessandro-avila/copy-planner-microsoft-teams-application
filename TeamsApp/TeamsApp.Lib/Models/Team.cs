using Newtonsoft.Json;

namespace TeamsAppLib.Models
{
    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; protected set; }
        [JsonProperty("description")]
        public string Description { get; protected set; }
        [JsonProperty("isArchived")]
        public bool IsArchived { get; protected set; }
        public Channel[] Channels { get; set; }
    }
}
