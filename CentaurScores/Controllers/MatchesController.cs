using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("match")]
    public class MatchesController
    {
        private readonly IMatchRepository matchRepository;

        public MatchesController(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        /// <summary>
        /// Returns the configuuration for all matches in the system, alphabetically ordered by their code.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MatchModel>>> GetMatches()
        {
            return await matchRepository.FindMatches();
        }

        /// <summary>
        /// Returns the data for one specific match identified by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> GetMatch([FromRoute] int id)
        {
            return await matchRepository.GetMatch(id);
        }

        /// <summary>
        /// Updates the metadata for a specific match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> UpdateMatch([FromRoute] int id, [FromBody] MatchModel match)
        {
            return await matchRepository.UpdateMatch(id, match);
        }

        /// <summary>
        /// Deletes the specified match.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteMatch([FromRoute] int id)
        {
            return await matchRepository.DeleteMatch(id);
        }

        /// <summary>
        /// Creates a new metadata record for a match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> CreateMatch([FromBody] MatchModel match)
        {
            return await matchRepository.CreateMatch(match);
        }

        /// <summary>
        /// Returns the data for one specific match.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel?>> GetActiveMatch()
        {
            return await matchRepository.GetActiveMatch();
        }

        /// <summary>
        /// Marks one match as the currently active match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPut("{id}/active/{isActive}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> ActivateMatch([FromRoute] int id, [FromRoute] bool isActive)
        {
            return await matchRepository.ActivateMatch(id, isActive);
        }
    }
}
