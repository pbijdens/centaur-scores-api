using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// Required interface for services providing rulesets. Register these in the RulesetRegistrations class.
    /// </summary>
    public interface IRuleService
    {
        /// <summary>
        /// Return a list of all rulesets that are offered by the service, making sure rulsets that can co-exist in a single competition
        /// both specify the same GroupName.
        /// </summary>
        public Task<List<RulesetModel>> GetSupportedRulesets();

        /// <summary>
        /// Using this ruleset model, calculate the results for a single match.
        /// </summary>
        public Task<MatchResultModel> CalculateSingleMatchResult(int matchId);

        /// <summary>
        /// Using the rulest models defined in this module, calculate the results for an entire competition.
        /// </summary>
        public Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId);
    }
}
