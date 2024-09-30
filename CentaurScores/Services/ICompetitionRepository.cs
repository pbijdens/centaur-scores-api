using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    /// <summary>
    ///  All competition-related operations
    /// </summary>
    public interface ICompetitionRepository
    {
        Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model);
        Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model);
        Task<int> DeleteParticipantList(int listId);
        Task<int> DeleteParticipantListMember(int listId, int memberId);
        Task<ParticipantListModel?> GetParticipantList(int listId);
        Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId);
        Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId);
        Task<List<ParticipantListModel>> GetParticipantLists();
        Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model);
        Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model);
    }
}
