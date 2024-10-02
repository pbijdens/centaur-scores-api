using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    public interface IMatchRepository
    {
        Task<MatchModel> ActivateMatch(int id, bool isActive);
        Task<MatchModel> CreateMatch(MatchModel match);
        Task<bool> DeleteMatch(int id);
        Task<List<MatchModel>> FindMatches();
        Task<MatchModel?> GetActiveMatch();
        Task<MatchModel> GetMatch(int id);
        Task<List<ParticipantModel>> GetParticipantsForMatch(int id);
        Task<List<ParticipantModel>> GetParticipantsForMatchByDeviceID(int id, string deviceID);
        Task<bool> TransferParticipantForMatchToDevice(int id, int participantId, string targetDeviceID, string lijn);
        Task<MatchModel> UpdateMatch(int id, MatchModel match);
        Task<int> UpdateParticipantsForMatch(int id, string deviceID, List<ParticipantModel> match);
    }
}