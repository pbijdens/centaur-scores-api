using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// Not the most descrfiptive name but I like this one.
    /// </summary>
    public interface IRuleService
    {
        public Task<List<RulesetModel>> GetSupportedRulesets();

        public Task<MatchResultModel> CalculateSingleMatchResult(int matchId);

        public Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId);
    }
}
