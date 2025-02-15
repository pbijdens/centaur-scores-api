using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CentaurScores.Services
{
    /// <summary>
    /// Service for dealing with tokens.
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class TokenService(IConfiguration configuration, IOptions<AppSettings> appSettings) : ITokenService
    {
        private readonly AppSettings appSettings = appSettings.Value;

        /// <summary>
        /// Claim type for the numeric user ID
        /// </summary>
        public const string IdClaim = "userid";
        /// <summary>
        /// 'Escape' semicolons in ACL names by replacing them with this token when returning ACLs as a semi-colon separated list.
        /// </summary>
        public const string SemicolonReplacedBy = @":";

        /// <inheritdoc/>
        public async Task<string> GenerateJwtToken(string userID, string loginSecret)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                AccountEntity? account = await db.Accounts.Include(acc => acc.ACLs).SingleOrDefaultAsync(a => a.Username == userID);
                if (account != null && account.SaltedPasswordHash.Length > 4)
                {
                    string salt = account.SaltedPasswordHash[..4];
                    string expectedPassword = salt + CalculateSha256Hash(salt + loginSecret);
                    if (expectedPassword == account.SaltedPasswordHash)
                    {
                        return await GenerateJwtToken(account);
                    }
                }
            }
            throw new UnauthorizedAccessException($"Bad credentials");
        }

        /// <summary>
        /// Calculates a hash as hex string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CalculateSha256Hash(string input)
        {
            byte[] data = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary></summary>
        public const string TokenIdentifier = "token-id";
        /// <summary></summary>
        public const string UsernameClaim = "username";
        /// <summary></summary>
        public const string IsAdministratorClaim = "is-administrator";
        /// <summary></summary>
        public const string GroupsClaim = "acls";

        private async Task<string> GenerateJwtToken(AccountEntity account)
        {
            Guid tokenId = Guid.NewGuid();
            Dictionary<string, string> claims = new Dictionary<string, string>() {
                { TokenIdentifier, $"{tokenId}" },
                { UsernameClaim, account.Username },
                { IsAdministratorClaim, (account.ACLs ?? []).Any(acl => acl.Id == appSettings.AdminACLId) ? "true" : "false" },
                { GroupsClaim, string.Join(";", (account.ACLs ?? []).Select(a => $"{a.Name}".Replace(";",SemicolonReplacedBy))) }, // really crappy escape but everyone using this string in a groupname deserves what they get
            };

            // TODO: Register tokenId in a fast datastore, link it to the user, and add some of the device info, allowing the user to
            //       later-on  invalidate this specific token. Reject unlisted tokens. Adds another layer of security that we don't
            //       really need right now.

            foreach (var acl in account.ACLs ?? [])
            {
                claims[$"{GroupsClaim}-{acl.Name}"] = "true";
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([new Claim(IdClaim, $"{account.Id}")]),
                    Claims = new Dictionary<string, object>(claims.Select(a => new KeyValuePair<string, object>(a.Key, a.Value))),
                    Expires = DateTime.UtcNow.AddMinutes(150),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}

