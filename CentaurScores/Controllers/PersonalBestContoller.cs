using CentaurScores.Attributes;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for maintaining peronal best scores for participants.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("/participantlists/{memberListId}/pbl")]
    public class PersonalBestContoller(IPersonalBestService personalBestService)
    {
        /// <summary>
        /// Returns all personal best lists in the context of the member list.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <returns>A list of participant list models.</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PersonalBestListModel>>> GetPersonalBestLists([FromRoute] int memberListId)
        {
            return await personalBestService.GetPersonalBestLists(memberListId);
        }

        /// <summary>
        /// Returns a single personal best list, with entries.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID</param>
        /// <returns>The participant list model for this list.</returns>
        [HttpGet("{personalBestListId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonalBestListModel>> GetPersonalBestList([FromRoute] int memberListId, [FromRoute] int personalBestListId)
        {
            return await personalBestService.GetPersonalBestList(memberListId, personalBestListId);
        }

        /// <summary>
        /// Update the personal best list's metadata.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID</param>
        /// <param name="model">The new metadata.</param>
        /// <returns>A copy of the updated object.</returns>
        [HttpPut("{personalBestListId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<PersonalBestListModel?>> UpdatePersonalBestList([FromRoute] int memberListId, [FromRoute] int personalBestListId, [FromBody] PersonalBestListModel model)
        {
            return await personalBestService.UpdatePersonalBestList(memberListId, model);
        }

        /// <summary>
        /// Delete the personal best list with this ID.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID.</param>
        /// <returns>Number of deleted objects.</returns>
        [HttpDelete("{personalBestListId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> DeletePersonalBestList([FromRoute] int memberListId, [FromRoute] int personalBestListId)
        {
            return await personalBestService.DeletePersonalBestList(memberListId, personalBestListId);
        }

        /// <summary>
        /// Create a new personal best  list.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="model">The lits's metadata, will ignore entries.</param>
        /// <returns>A copy of the newly created list, with the new ID.</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<PersonalBestListModel?>> CreatePersonalBestList([FromRoute] int memberListId, [FromBody] PersonalBestListModel model)
        {
            return await personalBestService.CreatePersonalBestList(memberListId, model);
        }

        /// <summary>
        /// Returns all entries on a personal best list, ordered by discipline, then score descending..
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID</param>
        /// <returns>All memeber in the form of member models.</returns>
        [HttpGet("{personalBestListId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PersonalBestListEntryModel>>> GetPersonalBestListEntrys([FromRoute] int memberListId, [FromRoute] int personalBestListId)
        {
            return (await personalBestService.GetPersonalBestList(memberListId, personalBestListId)).Entries;
        }

        /// <summary>
        /// Returns the personal best record for a single member, given the ID of the record.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID</param>
        /// <param name="memberId">The list ID</param>
        /// <returns>All memeber in the form of member models.</returns>
        [HttpGet("{personalBestListId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonalBestListEntryModel>> GetPersonalBestListEntry([FromRoute] int memberListId, [FromRoute] int personalBestListId, [FromRoute] int memberId)
        {
            return await personalBestService.GetPersonalBestListrEntry(memberListId, personalBestListId, memberId);
        }

        /// <summary>
        /// Updates a single personal best list entry by ID.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The ID of the list.</param>
        /// <param name="memberId">The ID of the record that is to be updated.</param>
        /// <param name="model">The new metadata model.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{personalBestListId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<PersonalBestListEntryModel?>> UpdatePersonalBestListEntry([FromRoute] int memberListId, [FromRoute] int personalBestListId, [FromRoute] int memberId, [FromBody] PersonalBestListEntryModel model)
        {
            return await personalBestService.UpdatePersonalBestListEntry(memberListId, personalBestListId, model);
        }

        /// <summary>
        /// Deleted a single personal best list entry by ID.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The ID of the list.</param>
        /// <param name="memberId">The ID of the record that is to be updated.</param>
        /// <param name="model">The new metadata model.</param>
        /// <returns>The updated model.</returns>
        [HttpDelete("{personalBestListId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> DeletePersonalBestListEntry([FromRoute] int memberListId, [FromRoute] int personalBestListId, [FromRoute] int memberId)
        {
            return await personalBestService.DeletePersonalBestListEntry(memberListId, personalBestListId, memberId);
        }

        /// <summary>
        /// Add a new personal best record to a list.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <param name="personalBestListId">The list ID</param>
        /// <param name="model">The metadata model for the member, the id is ignored.</param>
        /// <returns>The newly added model with its ID.</returns>
        [HttpPost("{personalBestListId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonalBestListEntryModel?>> CreatePersonalBestListEntry([FromRoute] int memberListId, [FromRoute] int personalBestListId, [FromBody] PersonalBestListEntryModel model)
        {
            return await personalBestService.CreatePersonalBestListEntry(memberListId, personalBestListId, model);
        }

        /// <summary>
        /// Scan for new records and suggest additions to the various lists. Will consider all competition scores in the system. May take some time to complete.
        /// </summary>
        /// <param name="memberListId">The parent participant list ID.</param>
        /// <returns>The newly added model with its ID.</returns>
        [HttpGet("suggestions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<List<NewPersonalBestModel>>> ScanForRecords([FromRoute] int memberListId)
        {
            return await personalBestService.CalculateUpdatedRecords(memberListId);
        }
    }
}
