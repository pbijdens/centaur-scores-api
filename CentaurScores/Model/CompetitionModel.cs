﻿namespace CentaurScores.Model
{
    public class CompetitionModel
    {
        /// <summary>
        /// Identifies this competition.
        /// </summary>
        public int? Id { get; set; } = null;
        /// <summary>
        /// NAme of the competition.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Identifies the rulesest that are in use, if any. Limits options for targets, keyboards, number of ends, arrows per end etc. for new matches added to this competition to only those that are included in the ruleset definition.
        /// </summary>
        public string? RulesetGroupName { get; set; } = null;
        /// <summary>
        /// Start date of the competition, informative.
        /// </summary>
        public DateOnly? StartDate { get; set; } = null;
        /// <summary>
        /// End date of the competition, informative.
        /// </summary>
        public DateOnly? EndDate { get; set; } = null;
        /// <summary>
        /// List of matches that have been added to this competition.
        /// </summary>
        public List<MatchMetadataModel> Matches { get; set; } = [];
        /// <summary>
        /// Fixed partipant list to be used for matches in this competition. Only archers on this list are allowed to take part in the competition.
        /// </summary>
        public ParticipantListMetadataModel? ParticipantsList { get; set; } = null;
    }
}