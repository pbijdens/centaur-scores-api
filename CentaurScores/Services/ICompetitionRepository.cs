using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    /// <summary>
    ///  All competition-related operations, more a service than a repository.
    /// </summary>
    public interface ICompetitionRepository
    {
        /// <summary>
        /// Create a new competition entity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CompetitionModel?> CreateCompetition(CompetitionModel model);
        /// <summary>
        /// Delete a competition.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        Task<int> DeleteCompetition(int competitionId);
        /// <summary>
        /// Get a single competition.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        Task<CompetitionModel?> GetCompetition(int competitionId);
        /// <summary>
        /// Get all competitions/
        /// </summary>
        /// <returns></returns>
        Task<List<CompetitionModel>> GetCompetitions();
        /// <summary>
        /// Get all supported rulesets
        /// </summary>
        /// <returns></returns>
        Task<List<RulesetModel>> GetRulesets();
        /// <summary>
        /// Updates a competition's metadata.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CompetitionModel?> UpdateCompetition(int competitionId, CompetitionModel model);
        /// <summary>
        /// Calculate the scores for a competition using the applicable ruleset.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        Task<CompetitionResultModel?> CalculateCompetitionResult(int competitionId);
        /// <summary>
        /// Calculate the score for a single match based on its ruleset.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<MatchResultModel?> CalculateSingleMatchResult(int matchId);
    }
}
