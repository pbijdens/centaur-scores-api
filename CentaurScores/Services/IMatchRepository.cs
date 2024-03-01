using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    public interface IMatchRepository
    {
        Task<ActionResult<MatchModel>> CreateMatch(MatchModel match);
        Task<ActionResult<MatchModel>> DeleteMatch(int id);
        Task<ActionResult<List<MatchModel>>> FindMatches(int id);
        Task<ActionResult<MatchModel>> GetMatch(int id);
        Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatch(int id);
        Task<ActionResult<MatchModel>> UpdateMatch(int id, MatchModel match);
        Task<ActionResult<MatchModel>> UpdateParticipantsForMatch(int id, List<ParticipantModel> match);
    }
}