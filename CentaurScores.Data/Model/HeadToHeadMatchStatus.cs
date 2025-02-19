namespace CentaurScores.Model
{
    public class HeadToHeadMatchStatus
    {
        public ParticipantData? Participant { get; set; }

        public List<int?> EndScores { get; set; } = [];

        public int Score { get; set; } = 0;

        public bool IsWinner { get; set; }
    }
}