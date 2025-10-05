using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for interactiunbg with rulesets.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("/rulesets")]
    public class RulesetController(ICompetitionService competitionService)
    {
        /// <summary>
        /// Returns all available pre-defined rulesets for this software. A ruleset is a group of match types and
        /// can (should) be applied to a competition. If it is, all matches in the competition will be of a type
        /// that is supported by the ruleset.
        /// </summary>
        /// <returns>All available rulesets.</returns>
        [HttpGet()]
        [HttpGet("/list/{listId}/rulesets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RulesetModel>>> GetRulesets(int? listId)
        {
            return await competitionService.GetRulesets(listId);
        }
    }
}
