using CentaurScores.Model;

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
        Task<List<CompetitionModel>> GetCompetitions(int? listId);
        /// <summary>
        /// Updates a competition's metadata.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CompetitionModel?> UpdateCompetition(int competitionId, CompetitionModel model);
    }
}
