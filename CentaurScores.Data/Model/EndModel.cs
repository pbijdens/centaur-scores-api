namespace CentaurScores.Model
{
    /// <summary>
    /// Part of a participant model for a match, score for a single end.
    /// </summary>
    public class EndModel
    {
        /// <summary>
        /// Identifier for this end, not used curently.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The sum of all arrows for this end or null if no arrows were shot.
        /// </summary>
        public int? Score { get; set; }
        /// <summary>
        /// Ordered list of all arrows for an end, null if not shot, the arrow score otherwise.
        /// </summary>
        public List<int?> Arrows { get; set; } = [];
        /// <summary>
        /// The round for which this score is registered. Should automatically be set to the active round.
        /// </summary>
        public int Round { get; set; } = 0;
    }
}
