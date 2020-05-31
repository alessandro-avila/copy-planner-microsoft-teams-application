using Newtonsoft.Json;

namespace TeamsAppLib.Models
{
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }
        [JsonProperty("description")]
        public string Description { get; protected set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; protected set; }
    }
}
