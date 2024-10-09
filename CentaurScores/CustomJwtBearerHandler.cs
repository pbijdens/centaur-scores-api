using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CentaurScores.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CentaurScores
{
    /// <summary>
    /// Adapted from https://sardarmudassaralikhan.medium.com/custom-jwt-handler-in-asp-net-core-7-web-api-c3af9e423b52: 
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    public class CustomJwtBearerHandler(IOptions<AppSettings> appSettings, IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ILogger<CustomJwtBearerHandler> log) 
        : JwtBearerHandler(options, logger, encoder)
    {
        private readonly AppSettings appSettings = appSettings.Value;

        /// <inheritdoc/>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get the token from the Authorization header
            if (!Context.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            // Check to see if the token makes some sense 
            var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            await Task.FromResult(0);

            // Validate the token
            string token = authorizationHeader["Bearer ".Length..].Trim();

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clock skew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            if (null == validatedToken)
            {
                log.LogWarning($"Validated token is null");
            }

            // Set the authentication result with the claims from the API response          
            ClaimsPrincipal principal = ToClaimsPrincipal(token);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, "CustomJwtBearer"));
        }


        private static ClaimsPrincipal ToClaimsPrincipal(string receivedToken)
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken? token = handler.ReadToken(receivedToken) as JwtSecurityToken;

            ClaimsIdentity claimsIdentity = new(token?.Claims ?? [], "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}