
namespace CentaurScores.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(string userID);
        Task<string> GenerateJwtToken(string userID, string loginSecret);
    }
}
