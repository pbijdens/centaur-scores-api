using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    /// <summary>
    ///  All competition-related operations
    /// </summary>
    public interface ICompetitionRepository
    {
        Task<CompetitionModel?> CreateCompetition(CompetitionModel model);
        Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model);
        Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model);
        Task<int> DeleteCompetition(int competitionId);
        Task<int> DeleteParticipantList(int listId);
        Task<int> DeleteParticipantListMember(int listId, int memberId);
        Task<CompetitionModel?> GetCompetition(int competitionId);
        Task<List<CompetitionModel>> GetCompetitions();
        Task<ParticipantListModel?> GetParticipantList(int listId);
        Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId);
        Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId);
        Task<List<ParticipantListModel>> GetParticipantLists();
        Task<List<RulesetModel>> GetRulesets();
        Task<CompetitionModel?> UpdateCompetition(int competitionId, CompetitionModel model);
        Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model);
        Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model);
        Task<CompetitionResultModel?> CalculateCompetitionResult(int competitionId);
        Task<MatchResultModel?> CalculateSingleMatchResult(int matchId);
    }
}
