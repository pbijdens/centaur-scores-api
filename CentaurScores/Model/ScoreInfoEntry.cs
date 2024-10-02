namespace CentaurScores.Model
{
    public class ScoreInfoEntry {
        public bool IsDiscarded { get; set; } = false;
        public int Score { get; set; } = 0;
        public string Info { get; set; } = string.Empty;
    }
}
