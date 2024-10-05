namespace CentaurScores.Model
{
    /// <summary>
    /// Models the results for a competition; currenlty uses teh same models as matches and therefore is a sub-class.
    /// </summary>
    public class CompetitionResultModel
    {
        public string Name { get; set; } = string.Empty;
        public string RulesetGroupName { get; set; } = string.Empty;
        public string RulesetParametersJSON { get; set; } = string.Empty;

        public List<GroupInfo> Groups {  get; set; } = [];
        public List<GroupInfo> Subgroups { get; set; } = [];

        public Dictionary<string, CompetitionResultHeader> Matches { get; set; } = [];

        public List<CompetitionResultEntry> Ungrouped { get; set; } = [];
        public Dictionary<string, List<CompetitionResultEntry>> ByClass { get; set; } = [];
        public Dictionary<string, Dictionary<string, List<CompetitionResultEntry>>> BySubclass { get; set; } = [];
    }
}
