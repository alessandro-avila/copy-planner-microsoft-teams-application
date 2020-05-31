using Newtonsoft.Json;
using System;

namespace TeamsAppLib.Models
{
    public class PlannerTask
    {
        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }

        [JsonProperty("planId")]
        public string PlanId { get; set; }

        [JsonProperty("bucketId")]
        public string BucketId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("orderHint")]
        public string OrderHint { get; set; }

        [JsonProperty("assigneePriority")]
        public string AssigneePriority { get; set; }

        [JsonProperty("percentComplete")]
        public long PercentComplete { get; set; }

        [JsonProperty("startDateTime")]
        public object StartDateTime { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonProperty("dueDateTime")]
        public object DueDateTime { get; set; }

        [JsonProperty("hasDescription")]
        public bool HasDescription { get; set; }

        [JsonProperty("previewType")]
        public string PreviewType { get; set; }

        [JsonProperty("completedDateTime")]
        public object CompletedDateTime { get; set; }

        [JsonProperty("completedBy")]
        public object CompletedBy { get; set; }

        [JsonProperty("referenceCount")]
        public long ReferenceCount { get; set; }

        [JsonProperty("checklistItemCount")]
        public long ChecklistItemCount { get; set; }

        [JsonProperty("activeChecklistItemCount")]
        public long ActiveChecklistItemCount { get; set; }

        [JsonProperty("conversationThreadId")]
        public object ConversationThreadId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdBy")]
        public IdentitySet CreatedBy { get; set; }

        [JsonProperty("appliedCategories")]
        public AppliedCategories AppliedCategories { get; set; }

        [JsonProperty("assignments")]
        public AppliedCategories Assignments { get; set; }
    }
}
