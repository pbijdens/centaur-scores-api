namespace CentaurScores.Model
{
    /// <summary>
    /// Data that helps rendering the headers for competition result pages.
    /// </summary>
    public class CompetitionResultHeader
    {
        /// <summary>
        /// The code of the ruleset.
        /// </summary>
        public string Ruleset { get; set; } = string.Empty;
        /// <summary>
        /// List of match codes in use.
        /// </summary>
        public List<string> MatchCodes {  get; set; } = [];
        /// <summary>
        /// List of match names belonging to the match code at the same index.
        /// </summary>
        public List<string> MatchNames { get; set; } = [];
    }
}