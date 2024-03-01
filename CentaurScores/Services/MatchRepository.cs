using CentaurScores.Model;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Services
{
    public class MatchRepository : IMatchRepository
    {
        public async Task<ActionResult<MatchModel>> CreateMatch(MatchModel match)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<MatchModel>> DeleteMatch(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<List<MatchModel>>> FindMatches(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<MatchModel>> GetMatch(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatch(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<MatchModel>> UpdateMatch(int id, MatchModel match)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<MatchModel>> UpdateParticipantsForMatch(int id, List<ParticipantModel> match)
        {
            throw new NotImplementedException();
        }
    }
}
