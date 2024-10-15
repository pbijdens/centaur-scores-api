using CentaurScores.Attributes;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CentaurScoresAPI
{
    /// <summary>
    /// Endpoints for user-authentication.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("auth")]
    public class AuthenticationController(ITokenService tokenService) : ControllerBase
    {
        /// <summary>
        /// Send a username and a password to obtain a Bearer token that can be used to authorize for specific operations.
        /// </summary>
        /// <param name="model">Model with the requested login details</param>
        /// <returns>Either returns a valid JWT token or reports a failed status.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestModel model)
        {
            string result = await tokenService.GenerateJwtToken(model.Username, model.Password);
            return result;
        }

        /// <summary>
        /// Will return the claims for an authenticated user. Can also be used to test if a token is still valid.
        /// </summary>
        /// <returns></returns>
        [HttpGet("whoami")]
        [Authorize]
        public async Task<ActionResult<WhoAmIResponse>> GetLoggedInUser()
        {
            WhoAmIResponse result = new();
            if (HttpContext.User != null)
            {
                result.Id = int.Parse(HttpContext.User.Claims.Single(x => x.Type == TokenService.IdClaim).Value);
                HttpContext.User.Claims.ToList().ForEach(x => { result.Claims[x.Type] = x.Value; });
            }
            return await Task.FromResult(result);
        }
    }
}
