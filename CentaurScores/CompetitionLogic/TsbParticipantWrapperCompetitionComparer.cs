namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// Objects of this class can be used to compare the scores of two wrapped competition participants
    /// to determine their mutual positions in the competition results.
    /// </summary>
    public class TsbParticipantWrapperCompetitionComparer : IComparer<TsbParticipantWrapperCompetition>
    {
        /// <summary>Comperator interface, -1 x is smaller, 0 same, 1 x is larger</summary>
        public int Compare(TsbParticipantWrapperCompetition? x, TsbParticipantWrapperCompetition? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            if (x.TotalScore.CompareTo(y.TotalScore) != 0) return x.TotalScore.CompareTo(y.TotalScore);

            // TODO: NEED TIEBREAKER?
            return 0;
        }
    }
}
