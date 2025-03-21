﻿namespace CentaurScores.Model
{
    /// <summary>
    /// Metadata for a single match.
    /// </summary>
    public class MatchMetadataModel
    {
        /// <summary>
        /// The match ID.
        /// </summary>
        public int? Id { get; set; } = -1;
        /// <summary>
        /// Code for the match. Can be freely chosen and does not need to be unique. I used for sorting lists of matches, so YYYY-MM-DD may be a good choice.
        /// </summary>
        public string MatchCode { get; set; } = string.Empty;
        /// <summary>
        /// Name of the match.
        /// </summary>
        public string MatchName { get; set; } = string.Empty;
        /// <summary>
        /// Code of the ruleset record that is used when creating this match.
        /// </summary>
        public string? RulesetCode { get; set; } = null;
        /// <summary>
        /// True if this is the active match, false otherwise.
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// Flags that define the match properties.
        /// </summary>
        public uint MatchFlags { get; set; } = 0;
        /// <summary>
        /// When this is a multi-part round such as a final, one of the rounds is currently active. All scores are
        /// registered for that round only, and all tablet interaction is only for that round.
        /// </summary>
        public int ActiveRound { get; set; } = 0;
        /// <summary>
        /// Currently active round for this match, only relevant for matches where multiple rounds are
        /// available. Can be used to split a single match into multiple rounds, provided the score
        /// calculator can merge these rounds again. Initially only used for finals.
        /// </summary>
        public int NumberOfRounds { get; set; } = 4;
    }
}
