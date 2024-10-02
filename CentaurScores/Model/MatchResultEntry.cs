namespace CentaurScores.Model
{
    public class MatchResultEntry
    {
        public int Position { get; set; } = 1;
        public string ParticipantInfo { get; set; } = string.Empty;
        public ScoreInfoEntry[] ScoreInfo { get; set; } = [];
        public int Score { get; set; } = 0;
        public string TiebreakerInfo { get; set; } = string.Empty;
    }
}
