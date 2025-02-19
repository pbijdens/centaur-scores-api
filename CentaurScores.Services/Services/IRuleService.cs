using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.Services
{
    /// <summary>
    /// Required interface for services providing rulesets. Register these in the RulesetRegistrations class.
    /// 
    /// TODO: Should separate competiton logic from match logic and allow competitions using mixed formats of any kind.
    ///       The competiton logic should just be able to merge various matches.
    /// </summary>
    public interface IRuleService
    {
        /// <summary>
        /// Return a list of all rulesets that are offered by the service, making sure rulsets that can co-exist in a single competition
        /// both specify the same GroupName.
        /// </summary>
        Task<List<RulesetModel>> GetSupportedRulesets();

        /// <summary>
        /// Using this ruleset model, calculate the results for a single match.
        /// </summary>
        Task<MatchResultModel> CalculateSingleMatchResult(int matchId);

        /// <summary>
        /// Using the rulest models defined in this module, calculate the results for an entire competition.
        /// </summary>
        Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId);

        /// <summary>
        /// Returns true only if this rule service can be used to calculate the results for the match.
        /// </summary>
        /// <param name="matchEntity"></param>
        /// <returns></returns>
        Task<bool> SupportsMatch(MatchEntity matchEntity);

        /// <summary>
        /// Returns true only if this rule service can be used to calculate the results for the competition.
        /// </summary>
        /// <param name="competitionEntity"></param>
        /// <returns></returns>
        Task<bool> SupportsCompetition(CompetitionEntity competitionEntity);
    }
}
