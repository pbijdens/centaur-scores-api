using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("/rulesets")]
    public class RulesetController
    {
        private readonly ICompetitionRepository competitionRepository;

        public RulesetController(ICompetitionRepository competitionRepository)
        {
            this.competitionRepository = competitionRepository;
        }

        /// <summary>
        /// Returns all pre-defined rulesets for this organization.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RulesetModel>>> GetRulesets()
        {
            return await competitionRepository.GetRulesets();
        }
    }
}
