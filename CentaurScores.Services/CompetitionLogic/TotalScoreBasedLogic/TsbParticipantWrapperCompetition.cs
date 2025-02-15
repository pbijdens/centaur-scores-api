using CentaurScores.Model;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    /// <summary>
    /// Object that represents a single participant in a competition.
    /// </summary>
    public class TsbParticipantWrapperCompetition
    {
        /// <summary>
        /// Identifies the participant and the participant's class and subclass.
        /// </summary>
        public required ParticipantData ParticipantData;

        /// <summary>
        /// Grouped per ruleset, the scores.
        /// </summary>
        public Dictionary<string, List<ScoreInfoEntry?>> ScoresPerRuleset = [];

        /// <summary>
        /// Grouped per ruleset, the total of all non-discarded scores for that ruleset.
        /// </summary>
        public Dictionary<string, int> TotalScoresPerRuleset = [];

        /// <summary>
        /// Average score per arrow.
        /// </summary>
        public double PerArrowAverage { get; set; }

        /// <summary>
        /// The sum of the scores per ruleset.
        /// </summary>
        public int TotalScore = 0;
    }
}