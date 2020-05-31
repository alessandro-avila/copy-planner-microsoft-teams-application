using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamsAppLib.Models
{
    public class PlannerTaskDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("previewType")]
        public string PreviewType { get; set; }

        [JsonProperty("references")]
        public References References { get; set; }

        [JsonProperty("checklist")]
        public Dictionary<string, Checklist> Checklist { get; set; }
    }

    public class References
    {
    }

    public class Checklist
    {
        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }

        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("orderHint")]
        public string OrderHint { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public DateTimeOffset LastModifiedDateTime { get; set; }

        [JsonProperty("lastModifiedBy")]
        public IdentitySet LastModifiedBy { get; set; }
    }
}
