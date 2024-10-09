using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Controller for administrative functions.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("admin")]
    public class AdminController(IDatabaseServices databaseServices, IConfiguration configuration) : ControllerBase
    {

        /// <summary>
        /// Request a backup of the MySQL database. Configure the secret in the "BackupSecret" in the AppSettings for the service.
        /// </summary>
        /// <param name="secret">A secret that should match the AppSettings.BackupSecret value.</param>
        /// <returns>A backup file</returns>
        [HttpGet("backup/{secret}")]
        public async Task<FileStreamResult> GetMySQLBackup([FromRoute] string secret)
        {
            string expectedSecret = configuration.GetSection("AppSettings").GetValue<string>("BackupSecret") ?? string.Empty;
            if (expectedSecret != secret) throw new UnauthorizedAccessException($"Incorrect credentials");
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return File(
                await databaseServices.GetDatabaseBackup(),
                "text/plain", 
                $"centaur-scores-db-backup-{now.Year}-{now.Month}-{now.Day}.sql",
                false);
        }
    }
}
