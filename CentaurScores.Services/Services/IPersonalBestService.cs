using CentaurScores.Model;

namespace CentaurScores.Services
{
    /// <summary>
    /// Helpers for maintaining personal best scores for archers.
    /// </summary>
    public interface IPersonalBestService
    {
        /// <summary>
        /// Check all matches for which there is a personal best category defined on the member list, then
        /// show all match results that are not on the list yet.
        /// </summary>
        /// <param name="memberListId">The member list to run this process for.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Task<List<PersonalBestListModel>> GetPersonalBestLists(int memberListId);

        /// <summary>
        /// Creates a new list for personal best results for a specific type of competition.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="list">The parameters ot be used when creating the list</param>
        /// <returns></returns>
        Task<PersonalBestListModel> CreatePersonalBestList(int memberListId, PersonalBestListModel list);

        /// <summary>
        /// Creates a new entry in a personal list.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="personalBestListId">The ID of the list.</param>
        /// <param name="entry">The parameters ot be used when creating the list entry.</param>
        /// <returns></returns>
        Task<PersonalBestListEntryModel> CreatePersonalBestListEntry(int memberListId, int personalBestListId, PersonalBestListEntryModel entry);

        /// <summary>
        /// Deletes a list with all data and records.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="personalBestListId">The ID of the list</param>
        /// <returns></returns>
        Task<int> DeletePersonalBestList(int memberListId, int personalBestListId);

        /// <summary>
        /// Deletes a record from a list.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="personalBestListId">The ID of the list</param>
        /// <param name="personalBestListEntryId">The ID of the record</param>
        /// <returns></returns>
        Task<int> DeletePersonalBestListEntry(int memberListId, int personalBestListId, int personalBestListEntryId);

        /// <summary>
        /// Updates a personal best lists'metadata with the specified data. Ignored entries.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="list">The new data for the list</param>
        /// <returns></returns>
        Task<PersonalBestListModel> UpdatePersonalBestList(int memberListId, PersonalBestListModel list);

        /// <summary>
        /// Updates a single personal best list entry.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="personalBestListId">The ID of the list.</param>
        /// <param name="entry">The parameters ot be used when updating the list entry.</param>
        /// <returns></returns>
        Task<PersonalBestListEntryModel> UpdatePersonalBestListEntry(int memberListId, int personalBestListId, PersonalBestListEntryModel entry);

        /// <summary>
        /// Returns a single (complete) personal best list record.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <param name="personalBestListId"></param>
        /// <returns></returns>
        Task<PersonalBestListModel> GetPersonalBestList(int memberListId, int personalBestListId);
        
        
        /// <summary>
        /// For a member list check which personal best records need to be updated.
        /// </summary>
        /// <param name="memberListId"></param>
        /// <returns></returns>
        Task<List<NewPersonalBestModel>> CalculateUpdatedRecords(int memberListId);

        /// <summary>
        /// Return a single member from the list, by ID
        /// </summary>
        /// <param name="memberListId">Participant list ID</param>
        /// <param name="personalBestListId">Personal best list ID </param>
        /// <param name="memberId">Member ID</param>
        /// <returns> The member or throw error</returns>
        Task<PersonalBestListEntryModel> GetPersonalBestListrEntry(int memberListId, int personalBestListId, int memberId);
    }
}
