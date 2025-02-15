namespace CentaurScores.Model
{
    /// <summary>
    /// Model representing the results for a single participant for a single resultset in a competition.
    /// </summary>
    public class CompetitionRulesetResultEntry
    {
        /// <summary>
        /// The score entry for each match in this resultset in the competition, even the ones that the user 
        /// did not take part in, and the ones for which their score was discarded.
        /// </summary>
        public List<ScoreInfoEntry?> Scores { get; set; } = [];
        /// <summary>
        /// Sum of the scoring matches for this user.
        /// </summary>
        public int TotalScore { get; set; } = 0;
    }
}