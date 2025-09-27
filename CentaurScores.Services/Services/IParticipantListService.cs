using CentaurScores.Model;

namespace CentaurScores.Services
{
    /// <summary>
    ///  All competition-related operations, more a service than a repository.
    /// </summary>
    public interface IParticipantListService
    {
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
        /// Deactivate the given member, not deleting them but essentially hiding them when the list is qieried.
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="memberId">Member ID</param>
        /// <returns></returns>
        Task<int> DeactivateParticipantListMember(int listId, int memberId);
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
        /// <param name="includeInactiveLists">If true also inactive lists are returned.</param>
        /// <returns></returns>
        Task<List<ParticipantListModel>> GetParticipantLists(bool includeInactiveLists = false);

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
    }
}
