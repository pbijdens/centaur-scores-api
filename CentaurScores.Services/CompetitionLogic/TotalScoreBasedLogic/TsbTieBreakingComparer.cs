namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    /// <summary>
    /// Comparer for single match results, can be used tro decide mutual position in the ranking for participants.
    /// Typically just needs total score, if identical will break ties by counting arrow scores, from high to low.
    /// Requires the tiebreaker dictionary to already be populated for both objects.
    /// </summary>
    /// <remarks>
    /// This comparer will update the record!
    /// </remarks>
    public class TsbTieBreakingComparer : IComparer<TsbParticipantWrapperSingleMatch>
    {
        /// <summary>
        /// Standard compare-to interface.
        /// </summary>
        public int Compare(TsbParticipantWrapperSingleMatch? x, TsbParticipantWrapperSingleMatch? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            if (x.Score != y.Score) return x.Score.CompareTo(y.Score);

            // Scores are identical, break a tie.
            int result = CompareTiebreakerValues(x, y);

            return result;
        }

        private static int CompareTiebreakerValues(TsbParticipantWrapperSingleMatch x, TsbParticipantWrapperSingleMatch y)
        {
            // We know that keysx and keysy are identical, because the dictionary is pre-populated for all
            // participants with a key for all of the arrow values that are seen anywhere in this match.
            var keysx = x.Tiebreakers.Keys.OrderByDescending(x => x).ToList();
            var keysy = y.Tiebreakers.Keys.OrderByDescending(x => x).ToList();

            // Just iterate through all key  values from high till low intil we find an entry where the counts are not the same for x and y, 
            // the record with the highest count is higher up in the ranking.
            int result = 0;
            for (int idx = 0; idx < keysx.Count; idx++)
            {
                int arrow = keysx[idx];

                x.TiebreakerArrow = Math.Min(arrow, x.TiebreakerArrow); // update the TiebreakerArrow for both records to be smaller 
                y.TiebreakerArrow = Math.Min(arrow, y.TiebreakerArrow); // than or equal to the tiebreaker used
                if (x.Tiebreakers[arrow] != y.Tiebreakers[arrow])
                {
                    result = x.Tiebreakers[arrow].CompareTo(y.Tiebreakers[arrow]);
                    break;
                }
            }

            return result;
        }
    }


}

