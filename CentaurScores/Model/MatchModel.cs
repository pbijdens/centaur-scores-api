using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace CentaurScores.Model
{
    /// <summary>
    /// Model representing all data for a single match.
    /// </summary>
    public class MatchModel
    {
        /// <summary>
        /// The match ID.
        /// </summary>
        public int Id { get; set; } = -1;
        /// <summary>
        /// The device ID for which this data is requested or returned.
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;
        /// <summary>
        /// Code for the match. Can be freely chosen and does not need to be unique. I used for sorting lists of matches, so YYYY-MM-DD may be a good choice.
        /// </summary>
        public string MatchCode { get; set; } = string.Empty;
        /// <summary>
        /// Name of the match.
        /// </summary>
        public string MatchName { get; set; } = string.Empty;
        /// <summary>
        /// Number of ends in the match.
        /// </summary>
        public int NumberOfEnds { get; set; } = -1;
        /// <summary>
        /// Number of arrows shot per end.
        /// </summary>
        public int ArrowsPerEnd { get; set; } = -1;
        /// <summary>
        /// Not used.
        /// </summary>
        public bool AutoProgressAfterEachArrow { get; set; } = false;
        /// <summary>
        /// True if this is the active match.
        /// </summary>
        public bool IsActiveMatch { get; set; } = false;
        /// <summary>
        /// Dictionary with per Code for a Target a set of button definitions defining the potential score values that can be entered.
        /// </summary>
        public Dictionary<string, List<ScoreButtonDefinition>> ScoreValues { get; set; } = [];
        /// <summary>
        /// List of groups for this match.
        /// </summary>
        public List<GroupInfo> Groups { get; set; } = [];
        /// <summary>
        /// List of subgroups for this match.
        /// </summary>
        public List<GroupInfo> Subgroups { get; set; } = [];
        /// <summary>
        /// List of allowed targets for the match. Be sure to add a ScoreValues record for each type of target.
        /// </summary>
        public List<GroupInfo> Targets { get; set; } = [];
        /// <summary>
        /// List of names of the lijnen for the match (should be 1 char each), e.g. 'A', 'B', 'C', 'D'
        /// </summary>
        public List<string> Lijnen { get; set; } = [];
        /// <summary>
        /// Ruleset code for the match.
        /// </summary>
        public string? RulesetCode { get; set; } = null;
        /// <summary>
        /// Competition for which the match is organized.
        /// </summary>
        public CompetitionModel? Competition { get; set; } = null;
        /// <summary>
        /// Not used.
        /// </summary>
        public bool ChangedRemotely { get; set; } = false;
    }
}
