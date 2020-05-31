using Newtonsoft.Json;
using System;

namespace TeamsAppLib.Models
{
    public class RootElem<T>
        where T : class
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }
        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }
        [JsonProperty("value")]
        public T[] Values { get; set; }
    }
}