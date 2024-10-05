namespace CentaurScores.CompetitionLogic
{
    public class TsbParticipantWrapperCompetitionComparer : IComparer<TsbParticipantWrapperCompetition>
    {
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
