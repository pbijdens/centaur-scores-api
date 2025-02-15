namespace CentaurScores.Model
{
    /// <summary>
    /// Result for a single match.
    /// </summary>
    public class MatchResultModel
    {
        /// <summary>
        /// Name of the match.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Code of the match.
        /// </summary>
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Ruleset that's currently in use for this match.
        /// </summary>
        public string Ruleset { get; set; } = string.Empty;

        /// <summary>
        /// All groups with scores.
        /// </summary>
        public List<GroupInfo> Groups { get; set; } = [];

        /// <summary>
        /// All subgroups with scores.
        /// </summary>
        public List<GroupInfo> Subgroups { get; set; } = [];

        /// <summary>
        /// Match scores by position from high to low, ignoring class and subclass.
        /// </summary>
        public List<MatchScoreForParticipant> Ungrouped { get; set; } = [];

        /// <summary>
        /// Match scores by position from high to low, grouped by class and ignoring subclass.
        /// </summary>
        public Dictionary<string, List<MatchScoreForParticipant>> ByClass { get; set; } = [];

        /// <summary>
        /// Match scores by position from high to low, grouped by class and subclass.
        /// </summary>
        public Dictionary<string, Dictionary<string, List<MatchScoreForParticipant>>> BySubclass { get; set; } = [];        
    }
}
