namespace CentaurScores.Model
{
    public class HeadToHeadScore
    {
        public int BracketNumber { get; set; }

        public HeadToHeadMatchStatus? Participant1 { get; set; }

        public HeadToHeadMatchStatus? Participant2 { get; set; }
    }
}