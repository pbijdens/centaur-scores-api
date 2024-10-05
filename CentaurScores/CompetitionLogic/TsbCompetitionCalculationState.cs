using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    public class TsbCompetitionCalculationState
    {
        public required CentaurScoresDbContext DB {  get; set; }
        public required CompetitionEntity CompetitionEntity { get; set; }
        public Dictionary<string, List<MatchResultModel>> MatchResultsByRuleset { get; set; } = [];
    }
}
