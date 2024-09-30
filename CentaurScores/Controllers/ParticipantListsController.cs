using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("/participantlists")]
    public class ParticipantListsController
    {
        private readonly ICompetitionRepository competitionRepository;

        public ParticipantListsController(ICompetitionRepository competitionRepository)
        {
            this.competitionRepository = competitionRepository;
        }

        /// <summary>
        /// Returns all pre-defined participants lists for this organization.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantListModel>>> GetParticipantLists()
        {
            return await competitionRepository.GetParticipantLists();
        }

        /// <summary>
        /// Returns a single participants list.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListModel?>> GetParticipantList([FromRoute] int listId)
        {
            return await competitionRepository.GetParticipantList(listId);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpPut("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListModel?>> UpdateParticipantList([FromRoute] int listId, [FromBody] ParticipantListModel model)
        {
            return await competitionRepository.UpdateParticipantList(listId, model);
        }

        /// <summary>
        /// Update the metadata for the partitipant list.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> UpdateParticipantList([FromRoute] int listId)
        {
            return await competitionRepository.DeleteParticipantList(listId);
        }

        /// <summary>
        /// Create a new participant list.
        /// </summary>
        /// <param name="model">The ID value in this model will be ignored.</param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListModel?>> CreateParticipantList([FromBody] ParticipantListModel model)
        {
            return await competitionRepository.CreateParticipantList(model);
        }

        ///

        /// <summary>
        /// Returns all members for a participant list.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{listId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ParticipantListMemberModel>>> GetParticipantListMembers([FromRoute] int listId)
        {
            return await competitionRepository.GetParticipantListMembers(listId);
        }

        /// <summary>
        /// Returns the metadata for a single member of a participant list.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListMemberModel?>> GetParticipantListMember([FromRoute] int listId, [FromRoute] int memberId)
        {
            return await competitionRepository.GetParticipantListMember(listId, memberId);
        }

        /// <summary>
        /// Updates the metadata for a single member of a participants list.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListMemberModel?>> UpdateParticipantListMember([FromRoute] int listId, [FromRoute] int memberId, [FromBody] ParticipantListMemberModel model)
        {
            return await competitionRepository.UpdateParticipantListMember(listId, memberId, model);
        }

        /// <summary>
        /// Delete a member from a participant-list.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{listId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> DeleteParticipantListMember([FromRoute] int listId, [FromRoute] int memberId)
        {
            return await competitionRepository.DeleteParticipantListMember(listId, memberId);
        }

        /// <summary>
        /// Add a new member to a participant list, the ID will be ignored.
        /// </summary>
        /// <returns></returns>
        [HttpPost("{listId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ParticipantListMemberModel?>> CreateParticipantListMember([FromRoute] int listId, [FromBody] ParticipantListMemberModel model)
        {
            return await competitionRepository.CreateParticipantListMember(listId, model);
        }

    }
}
