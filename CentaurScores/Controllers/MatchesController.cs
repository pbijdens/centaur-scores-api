using CentaurScores.Attributes;
using CentaurScores.Migrations;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Methods for managing matches and match results. Part of these endpoints are intended for mobile use.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("match")]
    public class MatchesController(IMatchRepository matchRepository
        , ICompetitionService competitionService
        , IFinalsService finalsService)
    {
        /// <summary>
        /// Returns the list of all matches in the system.
        /// </summary>
        /// <returns>All matches in the system</returns>
        [HttpGet()]
        [HttpGet("/list/{listId}/match")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MatchModel>>> GetMatches(int? listId)
        {
            return await matchRepository.FindMatches(listId);
        }

        /// <summary>
        /// Gets all data for a single match.
        /// </summary>
        /// <param name="id">The mmatch</param>
        /// <returns>A single match model</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel>> GetMatch([FromRoute] int id)
        {
            return await matchRepository.GetMatch(id);
        }

        /// <summary>
        /// Update all of a match's metadata, except participants.
        /// </summary>
        /// <param name="id">The match ID</param>
        /// <param name="match">The match metadata to be applied.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<MatchModel>> UpdateMatch([FromRoute] int id, [FromBody] MatchModel match)
        {
            return await matchRepository.UpdateMatch(id, match);
        }

        /// <summary>
        /// Deletes the specified match.
        /// </summary>
        /// <param name="id">The match ID</param>
        /// <returns>The number of matches deleted.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteMatch([FromRoute] int id)
        {
            return await matchRepository.DeleteMatch(id);
        }

        /// <summary>
        /// Creates a new match in the matches list.
        /// </summary>
        /// <param name="match">The metadata for the new match, participants are ignored.</param>
        /// <returns>A copy of the created model with the new ID.</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<MatchModel>> CreateMatch([FromBody] MatchModel match)
        {
            return await matchRepository.CreateMatch(match);
        }

        /// <summary>
        /// Returns the model data for the single activated match.
        /// </summary>
        /// <returns>A match model if there is an active match, or null otherwise.</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchModel?>> GetActiveMatch()
        {
            return await matchRepository.GetActiveMatch();
        }

        /// <summary>
        /// Marks one match as the currently active match, or clear that flag.
        /// </summary>
        /// <param name="id">The match ID</param>
        /// <param name="isActive">A boolean value indicateing if the match is active or not.</param>
        /// <returns>A copy of the updated model.</returns>
        [HttpPut("{id}/active/{isActive}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<MatchModel>> ActivateMatch([FromRoute] int id, [FromRoute] bool isActive)
        {
            return await matchRepository.ActivateMatch(id, isActive);
        }


        /// <summary>
        /// Returns results for a single match.
        /// </summary>
        /// <param name="id">The match ID</param>
        /// <returns>A match result object containign the results grouped three ways.</returns>
        [HttpGet("{id}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MatchResultModel>> GetMatchResults([FromRoute] int id)
        {
            MatchResultModel? result = await competitionService.CalculateSingleMatchResult(matchId: id);
            if (null == result) throw new ArgumentException("Bad match ID", nameof(id));
            return result;
        }
        
        /// <summary>
        /// Clear the remotely changed flag for a match.
        /// </summary>
        /// <param name="id">The match ID</param>
        /// <returns>A number</returns>
        [HttpDelete("{id}/remotelychanged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> ClearRemotelyChangedFlag([FromRoute] int id)
        {
            await matchRepository.ClearRemotelyChangedFlag(matchId: id);
            return 0;
        }

        /// <summary>
        /// Updates the current value for this UI setting.
        /// </summary>
        /// <param name="id">Match ID or -1 for default</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Value of the setting</param>
        /// <returns>Current value or null</returns>
        [HttpPut("{id}/setting/{name}/value/{value}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string?>> PutMatchUiSetting([FromRoute] int id, [FromRoute] string name, [FromRoute] string value)
        {
            return await matchRepository.UpdateMatchUiSetting(id, name, value);
        }

        /// <summary>
        /// Returns the current value for this UI setting.
        /// </summary>
        /// <param name="id">Match ID or -1 for default</param>
        /// <param name="name">Name of the setting</param>
        /// <returns>Current value or null</returns>
        [HttpGet("{id}/setting/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string?>> GetMatchUiSetting([FromRoute] int id, [FromRoute] string name)
        {
            return await matchRepository.GetMatchUiSetting(id, name);
        }

        /// <summary>
        /// Creates a new match in the matches list using a match definition object and a previous match.
        /// </summary>
        /// <param name="id">The match ID that was used as a template.</param>
        /// <param name="match">The metadata for the new match.</param>
        /// <returns>A copy of the created model with the new ID.</returns>
        [HttpPost("{id}/finals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<MatchModel>> CreateFinalsMatch([FromRoute] int id, [FromBody] FinalMatchDefinition match)
        {
            return await finalsService.CreateFromMatch(id, match);
        }

        /// <summary>
        /// Creates a new match in the matches list using a match definition object and a previous match.
        /// </summary>
        /// <param name="id">The match ID that was used as a template.</param>
        /// <returns>A copy of the created model with the new ID.</returns>
        [HttpPost("{id}/finals/nextround")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<bool>> CreateFinalsNextRound([FromRoute] int id)
        {
            await finalsService.GotoNextRound(id);
            return true;
        }

        /// <summary>
        /// Creates a new match in the matches list using a match definition object and a previous match.
        /// </summary>
        /// <param name="id">The match ID that was used as a template.</param>
        /// <returns>A copy of the created model with the new ID.</returns>
        [HttpPost("{id}/finals/nextround/undo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<bool>> CreateFinalsPrevRound([FromRoute] int id)
        {
            await finalsService.GotoPreviousRound(id);
            return true;
        }

        /// <summary>
        /// Creates a new match in the matches list using a match definition object and a previous match.
        /// </summary>
        /// <param name="id">The match ID that was used as a template.</param>
        /// <param name="bracket">Bracket ID</param>
        /// <param name="discipline">Discipline code</param>
        /// <param name="winner">Winner participant ID</param>
        /// <param name="loser">Loser participant ID</param>
        /// <returns>A copy of the created model with the new ID.</returns>
        [HttpPut("{id}/finals/win/{discipline}/{bracket}/{winner}/{loser}")]
        [HttpPut("{id}/finals/win/{bracket}/{winner}/{loser}")]
        [HttpPut("{id}/finals/win/{winner}/{loser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateFinalsBracketWinner([FromRoute] int id, [FromRoute] string? discipline, [FromRoute] int? bracket, [FromRoute] int winner, [FromRoute] int loser)
        {
            await finalsService.UpdateFinalsBracketWinner(id, discipline ?? string.Empty, bracket ?? -1, winner, loser);
            return true;
        }
    }
}
