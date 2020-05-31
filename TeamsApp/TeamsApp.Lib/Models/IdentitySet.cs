using Newtonsoft.Json;

namespace TeamsAppLib.Models
{
    public class IdentitySet
    {
        [JsonProperty("application")]
        public Identity Application { get; set; }
        public Identity Device { get; set; }
        public Identity Phone { get; set; }
        [JsonProperty("user")]
        public Identity User { get; set; }
    }
}
