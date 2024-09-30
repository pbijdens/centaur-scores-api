using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CentaurScores.Persistence
{
    public static class LocalMigrationExtensions
    {
        public static async Task MigrateDatabases(this WebApplication? app)
        {
            if (null == app)
            {
                return;
            }
            try
            {
                await using var scope = app.Services.CreateAsyncScope();
                IConfiguration? configuration = scope.ServiceProvider.GetService<IConfiguration>();
                if (null != configuration)
                {
                    using (CentaurScoresDbContext context = new(configuration))
                    {
                        await context.Database.MigrateAsync();
                    }
                }
                scope.ServiceProvider.GetService<ILogger<CentaurScoresDbContext>>()?.LogInformation("Database migrated succesfully.");
            }
            catch (Exception ex)
            {
                await using var scope = app.Services.CreateAsyncScope();
                scope.ServiceProvider.GetService<ILogger<CentaurScoresDbContext>>()?.LogError(ex, "Failed to apply migrations. Might need to INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)     VALUES ('20240303072601_InitialDatabase', '8.0.2');");

            }
        }
    }
}
