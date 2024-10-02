namespace CentaurScores.Model
{
    public class MatchMetadataModel
    {
        public int? Id { get; set; } = -1;
        public string MatchCode { get; set; } = string.Empty;
        public string MatchName { get; set; } = string.Empty;
        public string? RulesetCode { get; set; } = null;
        public bool IsActive { get; set; } = false;
    }
}
