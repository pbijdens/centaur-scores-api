namespace CentaurScores.Model
{
    public class CompetitionRulesetResultEntry
    {
        public List<ScoreInfoEntry?> Scores { get; set; } = [];
        public int TotalScore { get; set; } = 0;
    }
}