using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    public class TsbParticipantWrapperCompetition
    {
        public required string ParticipantName;
        public required string Group;
        public required string Subgroup;
        public Dictionary<string, List<ScoreInfoEntry?>> ScoresPerRuleset = [];
        public Dictionary<string, int> TotalScoresPerRuleset = [];
        public int TotalScore = 0;
    }
}