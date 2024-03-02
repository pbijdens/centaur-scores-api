using CentaurScores.Attributes;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CentaurScoresAPI
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService tokenService;

        public AuthenticationController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Send a username and a secret code to obtain a JWT token that can be used to authorize for specific operations.
        /// </summary>
        /// <param name="username">Username, as provided as part of your API key issuance</param>
        /// <param name="secret">Secret, as provided as part of your API key issuance</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Login([FromForm] string username, [FromForm] string secret)
        {
            return await tokenService.GenerateJwtToken(username, secret);
        }

        /// <summary>
        /// Can be used to create a new token thats a copy of the existing bearer token, but with its validity period reset.
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<string>> Refresh()
        {
            return await tokenService.GenerateJwtToken(HttpContext.Items["UserID"] as string ?? string.Empty);
        }


    }
}
