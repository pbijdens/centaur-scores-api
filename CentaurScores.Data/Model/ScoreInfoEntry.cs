
namespace CentaurScores.Model
{
    /// <summary>
    /// Entry indicating a single score for a match, plus how the score should be treated.
    /// </summary>
    public class ScoreInfoEntry {
        /// <summary>
        /// If set to true, the score was discarded and di not play a role in result calculation.
        /// </summary>
        public bool IsDiscarded { get; set; } = false;
        /// <summary>
        /// The score itself.
        /// </summary>
        public int Score { get; set; } = 0;
        /// <summary>
        /// Number of arrows.
        /// </summary>
        public int NumberOfArrows{ get; set; }
        /// <summary>
        /// Supporting information for the score, such as tiebreaker information or interesting details.
        /// If non-empty should be rendered.
        /// </summary>
        public string Info { get; set; } = string.Empty;

        /// <summary>
        /// Score components of the score, typically grouped by 10 arrows.
        /// </summary>
        public List<int> Scores { get; set; } = [];
    }
}
