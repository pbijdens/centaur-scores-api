using CentaurScores.Attributes;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for actions on competitions.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("/competitions")]
    public class CompetitionController(ICompetitionRepository competitionRepository)
    {

        /// <summary>
        /// Returns all competitions in the system.
        /// </summary>
        /// <returns>All competitions</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<CompetitionModel>>> GetCompetitions()
        {
            return await competitionRepository.GetCompetitions();
        }

        /// <summary>
        /// Returns the competition for the specified id.
        /// </summary>
        /// <param name="competitionId">Competition ID</param>
        /// <returns>A list of competitions</returns>
        [HttpGet("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionModel?>> GetCompetition([FromRoute] int competitionId)
        {
            return await competitionRepository.GetCompetition(competitionId);
        }

        /// <summary>
        /// Update a competition with the specified values.
        /// </summary>
        /// <param name="competitionId">Competition ID</param>
        /// <param name="model">A competition model containing the new metadata</param>
        /// <returns>The updated model</returns>
        [HttpPut("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<CompetitionModel?>> UpdateCompetition([FromRoute] int competitionId, [FromBody] CompetitionModel model)
        {
            return await competitionRepository.UpdateCompetition(competitionId, model);
        }

        /// <summary>
        /// Delete the competition, all matches and all results in the competition.
        /// </summary>
        /// <remarks>
        /// <para>The participant list is not deleted; It's shared.</para></remarks>
        /// <param name="competitionId">Competition ID</param>
        /// <returns>Zero if delete failed, a positive integer otherwise.</returns>
        [HttpDelete("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> DeleteCompetition([FromRoute] int competitionId)
        {
            return await competitionRepository.DeleteCompetition(competitionId);
        }

        /// <summary>
        /// Creates a new competition with the specified metadata.
        /// </summary>
        /// <param name="model">The metadata model, the ID in this model is ignored.</param>
        /// <returns>The new competition with the new ID.</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionModel?>> CreateCompetition([FromBody] CompetitionModel model)
        {
            return await competitionRepository.CreateCompetition(model);
        }

        /// <summary>
        /// Calculates the results for a competition and returns them in a result model.
        /// </summary>
        /// <param name="competitionId">Competition ID</param>
        /// <returns>Results for the competition.</returns>
        [HttpGet("{competitionId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionResultModel>> GetCompetitionResults([FromRoute] int competitionId)
        {
            CompetitionResultModel? result = await competitionRepository.CalculateCompetitionResult(competitionId);
            if (null == result) throw new ArgumentException("Bad competition ID", nameof(competitionId));
            return result;
        }
    }
}
