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
        /// Create a new participant list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model);
        /// <summary>
        /// Add a member to a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model);
        /// <summary>
        /// Delete a competition.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        Task<int> DeleteCompetition(int competitionId);
        /// <summary>
        /// Delete a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<int> DeleteParticipantList(int listId);
        /// <summary>
        /// Delete a member from a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<int> DeleteParticipantListMember(int listId, int memberId);
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
        /// Get all participants for a list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<ParticipantListModel?> GetParticipantList(int listId);
        /// <summary>
        /// Get a single member from a list by ID.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId);
        /// <summary>
        /// Get all members for a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId);
        /// <summary>
        /// Get all participant lists.
        /// </summary>
        /// <returns></returns>
        Task<List<ParticipantListModel>> GetParticipantLists();
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
        /// Update the metadata for a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model);
        /// <summary>
        /// Updates metadata for a single member in a participant list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="memberId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model);
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
