namespace CentaurScores.Model
{
    public class MatchResultModel
    {
        public List<MatchResultEntry> Ungrouped { get; set; } = [];
        public Dictionary<GroupInfo, List<MatchResultEntry>> ByClass { get; set; } = [];
        public Dictionary<GroupInfo, Dictionary<GroupInfo, List<MatchResultEntry>>> BySubclass { get; set; } = [];
    }
}
