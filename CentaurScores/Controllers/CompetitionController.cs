using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("/competitions")]
    public class CompetitionController
    {
        private readonly ICompetitionRepository competitionRepository;

        public CompetitionController(ICompetitionRepository competitionRepository)
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
        public async Task<ActionResult<List<CompetitionModel>>> GetCompetitions()
        {
            return await competitionRepository.GetCompetitions();
        }

        /// <summary>
        /// Returns a single participants list.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionModel?>> GetCompetition([FromRoute] int competitionId)
        {
            return await competitionRepository.GetCompetition(competitionId);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpPut("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionModel?>> UpdateCompetition([FromRoute] int competitionId, [FromBody] CompetitionModel model)
        {
            return await competitionRepository.UpdateCompetition(competitionId, model);
        }

        /// <summary>
        /// Update the metadata for the partitipant list.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{competitionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> DeleteCompetition([FromRoute] int competitionId)
        {
            return await competitionRepository.DeleteCompetition(competitionId);
        }

        /// <summary>
        /// Create a new participant list.
        /// </summary>
        /// <param name="model">The ID value in this model will be ignored.</param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionModel?>> CreateCompetition([FromBody] CompetitionModel model)
        {
            return await competitionRepository.CreateCompetition(model);
        }

        /// <summary>
        /// Returns a single participants list.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{competitionId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompetitionResultModel>> GetCompetitionResults([FromRoute] int competitionId)
        {
            CompetitionResultModel? result = await competitionRepository.CalculateCompetitionResult(competitionId);
            if (null == result) throw new ArgumentException(nameof(competitionId), "Bad competition ID");
            return result;
        }
    }
}
