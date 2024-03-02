using CentaurScores.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CentaurScores.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings appSettings;

        public TokenService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public async Task<string> GenerateJwtToken(string userID, string loginSecret)
        {
            if (loginSecret == appSettings.LoginSecret)
            {
                return await GenerateJwtToken(userID);
            }
            throw new InvalidOperationException($"Not authorized");
        }

        public async Task<string> GenerateJwtToken(string userID)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", userID) }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
