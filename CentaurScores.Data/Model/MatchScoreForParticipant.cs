
namespace CentaurScores.Model
{
    /// <summary>
    /// Single line in the match result for a participant.
    /// </summary>
    public class MatchScoreForParticipant
    {
        /// <summary>
        /// Position number. Numbers may be duplicated and skipped.
        /// </summary>
        public int Position { get; set; } = 1;
        
        /// <summary>
        /// The name of the participant plus markup indicating special circumstances (e.g. a draw).
        /// </summary>
        public string ParticipantInfo { get; set; } = string.Empty;

        /// <summary>
        /// Extended metadata for the participant.
        /// </summary>
        public ParticipantData ParticipantData { get; set; } = new();

        /// <summary>
        /// An array containing one single entry representing the participant's score for this match.
        /// </summary>
        public ScoreInfoEntry[] ScoreInfo { get; set; } = [];

        /// <summary>
        /// The total score for the 1 record(s) in the scoreinfo array...
        /// </summary>
        public int Score { get; set; } = 0;
        /// <summary>
        /// When using F1-style point scoring, contains the arrow score.
        /// </summary>
        public int ArrowScore { get; set; } = 0;
        /// <summary>
        /// When using F1-style point scoring, add the score for this week alongside the regular score.
        /// </summary>
        public int F1PointScore { get; set; } = 0;

        /// <summary>
        /// Average score per arrow.
        /// </summary>
        public double PerArrowAverage { get; set; } = 0;

        /// <summary>
        /// Average score per arrow.
        /// </summary>
        public double PrPerArrowAverage { get; set; } = 0;

        /// <summary>
        /// True if this may become a PR.
        /// </summary>
        public bool IsPR {  get; set; }

        /// <summary>
        /// PR Score
        /// </summary>
        public int PrScore { get; set; } = 0;

        /// <summary>
        /// Average score for the PR
        /// </summary>
        public double PrAverage { get; set; }

        /// <summary>
        /// If a tiebraker was needed to determing mutual position for this record, this line needs to be
        /// printed on the results-list.
        /// </summary>
        public string TiebreakerInfo { get; set; } = string.Empty;

        /// <summary>
        /// The score components, if any. Relevant for matches of 30+ arrows, grouping per 10
        /// </summary>
        public List<int> Scores { get; set; } = [];
    }
}
