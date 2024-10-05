namespace CentaurScores.Model
{
    public class CompetitionResultEntry
    {
        public string ParticipantInfo { get; set; } = string.Empty;

        public int Position { get; set; } = 0;

        public int TotalScore {  get; set; }

        public Dictionary<string, CompetitionRulesetResultEntry> PerRuleset { get; set; } = [];

    }
}