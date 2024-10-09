
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CentaurScores.Services
{
    /// <summary>
    /// Service for creating Oauth tokens
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generate a JWT token.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="loginSecret"></param>
        /// <returns></returns>
        Task<string> GenerateJwtToken(string userID, string loginSecret);
    }
}
