using TeamsAppLib.Enums;

namespace TeamsAppLib.Messages
{
    public class InCloneTeamMessage
    {
        public string TeamId { get; set; }
        private string Classification { get; set; }
        private string Description { get; set; }
        private string DisplayName { get; set; }
        private string MailNickname { get; set; }
        private ClonableTeamPart[] PartsToClone { get; set; }
        private TeamVisibilityType visibility { get; set; }
    }
}
