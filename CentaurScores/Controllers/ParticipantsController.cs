using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("match/{id}/participants")]
    public class ParticipantsController
    {
        private readonly IMatchRepository matchRepository;

        public ParticipantsController(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatch([FromRoute] int id)
        {
            return await matchRepository.GetParticipantsForMatch(id);
        }

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> UpdateMatch([FromRoute] int id, [FromBody] List<ParticipantModel> participants)
        {
            return await matchRepository.UpdateParticipantsForMatch(id, participants);
        }
    }
}
