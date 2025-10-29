using CentaurScores.Attributes;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for maintaining lists of potential match participants, to be uused when creating competitions.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("/participantlists")]
    public class ParticipantListsController(IParticipantListService participantListsRepository,
        IParticipantReportService participantReportService)
    {
        /// <summary>
        /// Returns all participant lists for this organization.
        /// </summary>
        /// <returns>A list of participant list models.</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantListModel>>> GetParticipantLists([FromQuery] bool inactive = false)
        {
            return await participantListsRepository.GetParticipantLists(inactive);
        }

        /// <summary>
        /// Returns a single participants list.
        /// </summary>
        /// <param name="listId">The list ID</param>
        /// <returns>The participant list model for this list.</returns>
        [HttpGet("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListModel?>> GetParticipantList([FromRoute] int listId)
        {
            return await participantListsRepository.GetParticipantList(listId);
        }

        /// <summary>
        /// Update the participant list's metadata.
        /// </summary>
        /// <param name="listId">The list ID</param>
        /// <param name="model">The new metadata.</param>
        /// <returns>A copy of the updated object.</returns>
        [HttpPut("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<ParticipantListModel?>> UpdateParticipantList([FromRoute] int listId, [FromBody] ParticipantListModel model)
        {
            return await participantListsRepository.UpdateParticipantList(listId, model);
        }

        /// <summary>
        /// Delete the participantr list with this ID.
        /// </summary>
        /// <param name="listId">The list ID.</param>
        /// <returns>Number of deleted objects.</returns>
        [HttpDelete("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> DeleteParticipantList([FromRoute] int listId)
        {
            return await participantListsRepository.DeleteParticipantList(listId);
        }

        /// <summary>
        /// Create a new participant list.
        /// </summary>
        /// <param name="model">The lits's metadata, will ignore entries.</param>
        /// <returns>A copy of the newly created list, with the new ID.</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListModel?>> CreateParticipantList([FromBody] ParticipantListModel model)
        {
            return await participantListsRepository.CreateParticipantList(model);
        }
        
        /// <summary>
        /// Returns all members for a participant list.
        /// </summary>
        /// <param name="listId">The list ID</param>
        /// <returns>All memeber in the form of member models.</returns>
        [HttpGet("{listId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantListMemberModel>>> GetParticipantListMembers([FromRoute] int listId)
        {
            return await participantListsRepository.GetParticipantListMembers(listId);
        }

        /// <summary>
        /// Returns the data record for a single list-member.
        /// </summary>
        /// <param name="listId">The list ID</param>
        /// <param name="memberId">The record ID for the member.</param>
        /// <returns></returns>
        [HttpGet("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListMemberModel?>> GetParticipantListMember([FromRoute] int listId, [FromRoute] int memberId)
        {
            return await participantListsRepository.GetParticipantListMember(listId, memberId);
        }

        /// <summary>
        /// Updates a single list member's metadata.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="memberId">The ID of the member record that is to be updated.</param>
        /// <param name="model">The new metadata model.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<ParticipantListMemberModel?>> UpdateParticipantListMember([FromRoute] int listId, [FromRoute] int memberId, [FromBody] ParticipantListMemberModel model)
        {
            return await participantListsRepository.UpdateParticipantListMember(listId, memberId, model);
        }

        /// <summary>
        /// Delete a member from a list.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="memberId">The ID of the member record that is to be deleted.</param>
        /// <returns></returns>
        [HttpDelete("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> DeleteParticipantListMember([FromRoute] int listId, [FromRoute] int memberId)
        {
            return await participantListsRepository.DeactivateParticipantListMember(listId, memberId);
        }

        /// <summary>
        /// Add a new member to a participant list.
        /// </summary>
        /// <param name="listId">The list ID</param>
        /// <param name="model">The metadata model for the member, the id is ignored.</param>
        /// <returns>The newly added model with its ID.</returns>
        [HttpPost("{listId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<ParticipantListMemberModel?>> CreateParticipantListMember([FromRoute] int listId, [FromBody] ParticipantListMemberModel model)
        {
            return await participantListsRepository.CreateParticipantListMember(listId, model);
        }

        [HttpGet("{listId}/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]

        public async Task<ActionResult<List<ParticipantReport>>> GetParticipantReportPerDiscipline([FromRoute] int listId)
        {
            return await participantReportService.GetParticipantReportPerDiscipline(listId);
        }
    }
}
