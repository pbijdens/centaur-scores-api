using CentaurScores.Model;

namespace CentaurScores.Services
{
    /// <summary>
    /// Match-related services.
    /// </summary>
    public interface IMatchRepository
    {
        /// <summary>
        /// Set this match as the active match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        Task<MatchModel> ActivateMatch(int id, bool isActive);
        /// <summary>
        /// Check if the device is listed for needing sync.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> CheckDeviceSynchronization(string deviceId);
        /// <summary>
        /// Clears the falg indicating the device needs sync.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task ClearDeviceSynchronization(string deviceId);
        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task ClearRemotelyChangedFlag(int matchId);
        /// <summary>
        /// Create a new match based on supplied data.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<MatchModel> CreateMatch(MatchModel match);
        /// <summary>
        /// Add a participant record to a match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task<ParticipantModel> CreateParticipantForMatch(int id, ParticipantModel participant);
        /// <summary>
        /// Delete a match.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteMatch(int id);
        /// <summary>
        /// Delete a participant form a match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        Task<int> DeleteParticipantForMatch(int id, int participantId);
        /// <summary>
        /// Get all matches using a weird verb that suggests the result is filtered but it's not.
        /// </summary>
        /// <returns></returns>
        Task<List<MatchModel>> FindMatches(int? listId);
        /// <summary>
        /// Returns the actrive match or null if none.
        /// </summary>
        /// <returns></returns>
        Task<MatchModel?> GetActiveMatch();
        /// <summary>
        /// Get this match's data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MatchModel> GetMatch(int id);

        /// <summary>
        /// Get a UI setting value for a specific match, or match -1 for lthe default live viuw
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<string?> GetMatchUiSetting(int id, string name);

        /// <summary>
        /// Update a UI setting value for a specific match, or match -1 for lthe default live viuw
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<string?> UpdateMatchUiSetting(int id, string name, string value);

        /// <summary>
        /// Return a single participant info structure in the context of a match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        Task<ParticipantModel> GetParticipantForMatch(int id, int participantId);
        /// <summary>
        /// Get all participants for a mcth.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<ParticipantModelV2>> GetParticipantsForMatch(int id);
        /// <summary>
        /// Gets a participant array for using it on a device, i.e. including one empty record for each unused lijn.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        Task<List<ParticipantModel>> GetParticipantsForMatchByDeviceID(int id, string deviceID);
        /// <summary>
        /// Transfer the participant from its current device to a new one./
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participantId"></param>
        /// <param name="targetDeviceID"></param>
        /// <param name="lijn"></param>
        /// <returns></returns>
        Task<bool> TransferParticipantForMatchToDevice(int id, int participantId, string targetDeviceID, string lijn);
        /// <summary>
        /// Update a match record with the supplied data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<MatchModel> UpdateMatch(int id, MatchModel match);
        /// <summary>
        /// Update a single participant and score for a match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participantId"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task<ParticipantModel> UpdateParticipantForMatch(int id, int participantId, ParticipantModel participant);
        /// <summary>
        /// Updates all participants and their scores for a device linked to a match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceID"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<int> UpdateParticipantsForMatch(int id, string deviceID, List<ParticipantModel> match);
    }
}