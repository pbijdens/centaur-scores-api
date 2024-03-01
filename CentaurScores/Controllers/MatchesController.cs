using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MatchModel>>> GetMatches([FromRoute] int id)
        {
            return await matchRepository.FindMatches(id);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> GetMatch([FromRoute] int id)
        {
            return await matchRepository.GetMatch(id);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> UpdateMatch([FromRoute] int id, [FromBody] MatchModel match)
        {
            return await matchRepository.UpdateMatch(id, match);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> DeleteMatch([FromRoute] int id)
        {
            return await matchRepository.DeleteMatch(id);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> CreateMatch([FromBody] MatchModel match)
        {
            return await matchRepository.CreateMatch(match);
        }

    }
}
