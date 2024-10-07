using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IDatabaseServices databaseServices;
        private readonly IConfiguration configuration;

        public AdminController(IDatabaseServices databaseServices, IConfiguration configuration)
        {
            this.databaseServices = databaseServices;
            this.configuration = configuration;
        }

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
