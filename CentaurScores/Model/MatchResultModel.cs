namespace CentaurScores.Model
{
    public class MatchResultModel
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Ruleset { get; set; } = string.Empty;
        public List<MatchResultEntry> Ungrouped { get; set; } = [];
        public Dictionary<string, List<MatchResultEntry>> ByClass { get; set; } = [];
        public Dictionary<string, Dictionary<string, List<MatchResultEntry>>> BySubclass { get; set; } = [];
        public List<GroupInfo> Groups { get; set; } = [];
        public List<GroupInfo> Subgroups { get; set; } = [];
    }
}
