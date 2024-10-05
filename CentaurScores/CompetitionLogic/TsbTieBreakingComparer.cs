namespace CentaurScores.CompetitionLogic
{
    public class TsbTieBreakingComparer : IComparer<TsbParticipantWrapperSingleMatch>
    {
        public int Compare(TsbParticipantWrapperSingleMatch? x, TsbParticipantWrapperSingleMatch? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            if (x.Score != y.Score) return x.Score.CompareTo(y.Score);

            int result = CompareTiebreakerValues(x, y);

            return result;
        }

        private static int CompareTiebreakerValues(TsbParticipantWrapperSingleMatch x, TsbParticipantWrapperSingleMatch y)
        {
            var keysx = x.Tiebreakers.Keys.OrderByDescending(x => x).ToList();
            var keysy = y.Tiebreakers.Keys.OrderByDescending(x => x).ToList();

            int result = 0;
            for (int idx = 0; idx < keysx.Count; idx++)
            {
                int arrow = keysx[idx];

                x.TiebreakerArrow = Math.Min(arrow, x.TiebreakerArrow);
                y.TiebreakerArrow = Math.Min(arrow, y.TiebreakerArrow);
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

