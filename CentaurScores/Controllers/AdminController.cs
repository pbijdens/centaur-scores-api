using CentaurScores.Attributes;
using CentaurScores.Model;
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
    public class AdminController(IDatabaseServices databaseServices, IConfiguration configuration, IConfigurationRepository configurationRepository) : ControllerBase
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

        /// <summary>
        /// Returns a configuration value given the spoecified key. Anonymoud access is allowed here.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("config/{key}")]
        public async Task<string?> GetConfiguration([FromRoute] string key)
        {
            return await configurationRepository.GetValue(key);
        }

        /// <summary>
        /// Updates a configuration setting. Requiires a logged-in user.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("config/{key}")]
        [Authorize]
        public async Task<bool> GetConfiguration([FromRoute] string key, [FromBody] ConfigurationRequestModel model)
        {
            return await configurationRepository.SetValue(key, model.Value);
        }
    }
}
