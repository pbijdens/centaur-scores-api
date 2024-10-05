namespace CentaurScores.Model
{
    public class CompetitionResultHeader
    {
        public string Ruleset { get; set; } = string.Empty;
        public List<string> MatchCodes {  get; set; } = [];
        public List<string> MatchNames { get; set; } = [];
    }
}