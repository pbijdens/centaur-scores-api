namespace CentaurScores.Model
{
    /// <summary>
    /// Models the results for a competition.
    /// </summary>
    public class CompetitionResultModel
    {
        /// <summary>
        /// Name of the competition.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Name of the ruleset group for this competition.
        /// </summary>
        public string RulesetGroupName { get; set; } = string.Empty;

        /// <summary>
        /// Optional parameters used in this competition.
        /// </summary>
        public string RulesetParametersJSON { get; set; } = string.Empty;

        /// <summary>
        /// List of groups with scores.
        /// </summary>
        public List<GroupInfo> Groups {  get; set; } = [];

        /// <summary>
        /// List of subgroups with scores.
        /// </summary>
        public List<GroupInfo> Subgroups { get; set; } = [];

        /// <summary>
        /// Defines per ruleset code the headers for the tables.
        /// </summary>
        public Dictionary<string, CompetitionResultHeader> Matches { get; set; } = [];

        /// <summary>
        /// Competition results ignoring groups and subgroups. One record per participant.
        /// </summary>
        public List<CompetitionScoreForParticipantModel> Ungrouped { get; set; } = [];

        /// <summary>
        /// Competition results per class, ignoring subclass.
        /// </summary>
        public Dictionary<string, List<CompetitionScoreForParticipantModel>> ByClass { get; set; } = [];

        /// <summary>
        /// Competition results by class and subclass.
        /// </summary>
        public Dictionary<string, Dictionary<string, List<CompetitionScoreForParticipantModel>>> BySubclass { get; set; } = [];
    }
}
