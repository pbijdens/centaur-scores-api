using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for maintaining the participants of a single match.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("match/{id}/participants")]
    public class MatchParticipantsController(IMatchRepository matchRepository)
    {
        /// <summary>
        /// Returns a complete list of all participants for a single match, ordered by name.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantModelV2>>> GetParticipantsForMatch([FromRoute] int id)
        {
            return await matchRepository.GetParticipantsForMatch(id);
        }
        
        /// <summary>
        /// Returns the list of participant entries that should be used on a device that's updating scores. Will return empty participant sctructures for currently not populated 'lijnen'.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="deviceID">Unique ID of the device that's requesting the list.</param>
        /// <returns>A list of participants with exactly as many positions in it as there are 'lijnen' for the match.</returns>
        [HttpGet("{deviceID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantModel>>> GetParticipantsForMatchByDevice([FromRoute] int id, [FromRoute] string deviceID)
        {
            return await matchRepository.GetParticipantsForMatchByDeviceID(id, deviceID);
        }

        /// <summary>
        /// Updates the participants for the specified match, also updating their scores. This will fail if the specified match is not the currently active match. 
        /// Will also fail if the device ID does not match the participant's device ID.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="deviceID">The devcice ID</param>
        /// <param name="participants">The list of participants, with a complete array of end-scores.</param>
        /// <returns>A number</returns>
        [HttpPut("{deviceID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> UpdateParticipantsForMatch([FromRoute] int id, [FromRoute] string deviceID, [FromBody] List<ParticipantModel> participants)
        {
            return await matchRepository.UpdateParticipantsForMatch(id, deviceID, participants);
        }

        /// <summary>
        /// Transfer a participant from one device onto another with all metadata and all scores. Does not require tha target 'lijn' to be available. 
        /// Will mark both devices as having received a remote model update.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="participantId">The record ID of the partitcipant.</param>
        /// <param name="targetDeviceID">The device ID for the target.</param>
        /// <param name="lijn">The 'lijn' at the target that will be occupied by this participant.</param>
        /// <returns>A boolean value indicating if the transfer succeeded.</returns>
        [HttpPost("{participantId}/transfer/{targetDeviceID}/{lijn}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> TransferParticipantToDevice([FromRoute] int id, [FromRoute] int participantId, [FromRoute] string targetDeviceID, [FromRoute] string lijn)
        {
            return await matchRepository.TransferParticipantForMatchToDevice(id, participantId, targetDeviceID, lijn);
        }

        /// <summary>
        /// Returns a single participant by ID with all metadata ans scoring data for the match..
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="participantId">The participant ID.</param>
        /// <returns></returns>
        [HttpGet("{participantId}/scoresheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantModel>> GetParticipantForMatch([FromRoute] int id, [FromRoute] int participantId)
        {
            return await matchRepository.GetParticipantForMatch(id, participantId);
        }

        /// <summary>
        /// Updates a single participant for a match. Will update metadata and scores. If the participant is linked to a device, the device will be flagged for needing synchronization.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="participantId">The participant ID.</param>
        /// <param name="participant">The new participant model.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{participantId}/scoresheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantModel>> UpdateParticipantForMatch([FromRoute] int id, [FromRoute] int participantId, [FromBody] ParticipantModel participant)
        {
            return await matchRepository.UpdateParticipantForMatch(id, participantId, participant);
        }

        /// <summary>
        /// Deletes a single participant for a match
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="participantId">The participant ID.</param>
        /// <returns>The number of records that were deleted.</returns>
        [HttpDelete("{participantId}/scoresheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> DeleteParticipantForMatch([FromRoute] int id, [FromRoute] int participantId)
        {
            return await matchRepository.DeleteParticipantForMatch(id, participantId);
        }

        /// <summary>
        /// Creates a new participant in a match.
        /// </summary>
        /// <param name="id">The match ID.</param>
        /// <param name="participant">Model data for the participant that is to be created.</param>
        /// <returns>The created model object with its ID.</returns>
        [HttpPost("scoresheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantModel>> CreateParticipantForMatch([FromRoute] int id, [FromBody] ParticipantModel participant)
        {
            return await matchRepository.CreateParticipantForMatch(id, participant);
        }
    }
}
