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

        /// <summary>
        /// Returns a complete list of all partiocipants for a match, ordered by name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatch([FromRoute] int id)
        {
            return await matchRepository.GetParticipantsForMatch(id);
        }
        
        /// <summary>
        /// Retuurns the list of participant entries that should be used on each device that's updating scores. Will return empty participant sctructures for currently not populated lijnen.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        [HttpGet("{deviceID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatchByDevice([FromRoute] int id, [FromRoute] string deviceID)
        {
            return await matchRepository.GetParticipantsForMatchByDeviceID(id, deviceID);
        }

        /// <summary>
        /// Updates the participants for the specified match, updating their scores. This will fail if the specified match is not the currewntly active match, because only that may be updated.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceID"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        [HttpPut("{deviceID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> UpdateParticipantsForMatch([FromRoute] int id, [FromRoute] string deviceID, [FromBody] List<ParticipantModel> participants)
        {
            return await matchRepository.UpdateParticipantsForMatch(id, deviceID, participants);
        }

        [HttpPost("{participantId}/transfer/{targetDeviceID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> TransferParticipantToDevice([FromRoute] int id, [FromRoute] int participantId, [FromRoute] string targetDeviceID)
        {
            return await matchRepository.TransferParticipantForMatchToDevice(id, participantId, targetDeviceID);
        }
    }
}
