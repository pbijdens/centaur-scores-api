namespace CentaurScores.Model
{
    /// <summary>
    /// Represents a competition.
    /// </summary>
    public class CompetitionModel
    {
        /// <summary>
        /// Identifies this competition.
        /// </summary>
        public int? Id { get; set; } = null;
        /// <summary>
        /// Name of the competition.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Identifies the rulesest that are in use, if any. Limits options for targets, keyboards, number of ends, arrows per end etc. for new matches added to this competition to only those that are included in the ruleset definition.
        /// </summary>
        public string? RulesetGroupName { get; set; } = null;
        /// <summary>
        /// Optional parameters to go with the ruleset.
        /// </summary>
        public string? RulesetParametersJSON { get; set; } = null;
        /// <summary>
        /// Start date of the competition, informative. YYYY-MM-DD
        /// </summary>
        public string? StartDate { get; set; } = null;
        /// <summary>
        /// End date of the competition, informative. YYYY-MM-DD
        /// </summary>
        public string? EndDate { get; set; } = null;
        /// <summary>
        /// List of matches that have been added to this competition.
        /// </summary>
        public List<MatchMetadataModel> Matches { get; set; } = [];
        /// <summary>
        /// Fixed partipant list to be used for matches in this competition. Only archers on this list are allowed to take part in the competition.
        /// </summary>
        public ParticipantListMetadataModel? ParticipantsList { get; set; } = null;
        /// <summary>
        /// Indicates if the competition is inactive or not.
        /// </summary>
        public bool IsInactive { get; set; } = false;
    }
}
