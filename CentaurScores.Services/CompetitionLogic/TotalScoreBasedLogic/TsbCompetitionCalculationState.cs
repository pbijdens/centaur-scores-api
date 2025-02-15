using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    /// <summary>
    /// State object used while calculating scores for a multi-match competition.
    /// </summary>
    public class TsbCompetitionCalculationState
    {
        /// <summary>
        /// Database context
        /// </summary>
        public required CentaurScoresDbContext DB { get; set; }
        /// <summary>
        /// Competition entity (no tracking) that's being processed.
        /// </summary>
        public required CompetitionEntity CompetitionEntity { get; set; }
        /// <summary>
        /// All match results in the competition, grouped by their basic ruleset (e.g. 25m3p / 18m3p)
        /// </summary>
        public Dictionary<string, List<MatchResultModel>> MatchResultsByRuleset { get; set; } = [];
    }
}
