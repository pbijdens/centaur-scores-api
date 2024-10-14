using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// For match result calculation, this contains all strate per participant needed to determine mutual
    /// order for every 2 participants, and to report on the result.
    /// </summary>
    public class TsbParticipantWrapperSingleMatch
    {
        /// <summary>
        /// Model data for the participant.
        /// </summary>
        public required ParticipantModel Participant;
        /// <summary>
        /// Core data needed for the result later.
        /// </summary>
        public required ParticipantData ParticipantData;
        /// <summary>
        /// All end scores
        /// </summary>
        public required IEnumerable<EndModel> Ends;
        /// <summary>
        /// Class or Group
        /// </summary>
        public required string ClassCode;
        /// <summary>
        /// Subclass or Subgroup
        /// </summary>
        public required string SubclassCode;
        /// <summary>
        /// Sum of all arrows in all ends.
        /// </summary>
        public int Score = 0;
        /// <summary>
        /// Tiebreaker scores, count of each arrow value.
        /// </summary>
        public Dictionary<int, int> Tiebreakers = [];
        /// <summary>
        /// Set during calculation when a tiebreaker is used lowest arrow score used in calculating hte tiebreaker.
        /// Can be used to determine which arrow counts need to be reported in the results (e.g. 10x10, 8x9, 3x8
        /// when the 8s were used as a tiebreaker).
        /// </summary>
        public int TiebreakerArrow = int.MaxValue;
        /// <summary>
        /// Per-arrow average for the user's PR in this match.
        /// </summary>
        public double PrPerArrowAverage = 0.0;
        /// <summary>
        /// Personal best.
        /// </summary>
        public int Pr = 0;
    }
}