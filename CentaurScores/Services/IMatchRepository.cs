using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    public interface IMatchRepository
    {
        Task<MatchModel> ActivateMatch(int id, bool isActive);
        Task<MatchModel> CreateMatch(MatchModel match);
        Task<ParticipantModel> CreateParticipantForMatch(int id, ParticipantModel participant);
        Task<bool> DeleteMatch(int id);
        Task<int> DeleteParticipantForMatch(int id, int participantId);
        Task<List<MatchModel>> FindMatches();
        Task<MatchModel?> GetActiveMatch();
        Task<MatchModel> GetMatch(int id);
        Task<ParticipantModel> GetParticipantForMatch(int id, int participantId);
        Task<List<ParticipantModelV2>> GetParticipantsForMatch(int id);
        Task<List<ParticipantModel>> GetParticipantsForMatchByDeviceID(int id, string deviceID);
        Task<bool> TransferParticipantForMatchToDevice(int id, int participantId, string targetDeviceID, string lijn);
        Task<MatchModel> UpdateMatch(int id, MatchModel match);
        Task<ParticipantModel> UpdateParticipantForMatch(int id, int participantId, ParticipantModel participant);
        Task<int> UpdateParticipantsForMatch(int id, string deviceID, List<ParticipantModel> match);
    }
}