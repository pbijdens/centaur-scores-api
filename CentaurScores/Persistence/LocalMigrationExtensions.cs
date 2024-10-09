using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// Helpers for migrating to newer database versions and for seeding msising data.
    /// </summary>
    public static class LocalMigrationExtensions
    {
        /// <summary>
        /// Migrate a database to the latest version of the data model. Be sure to use dotnet "ef migrations add MigrationName"!
        /// </summary>
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
                    using CentaurScoresDbContext context = new(configuration);
                    await context.Database.MigrateAsync();
                }
                scope.ServiceProvider.GetService<ILogger<CentaurScoresDbContext>>()?.LogInformation("Database migrated succesfully.");

                if (null != configuration)
                {
                    await SeedDatabaseIfNeeded(configuration);
                }

                scope.ServiceProvider.GetService<ILogger<CentaurScoresDbContext>>()?.LogInformation("Database seeded succesfully.");
            }
            catch (Exception ex)
            {
                await using var scope = app.Services.CreateAsyncScope();
                scope.ServiceProvider.GetService<ILogger<CentaurScoresDbContext>>()?.LogError(ex, "Failed to apply migrations. Might need to INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)     VALUES ('20240303072601_InitialDatabase', '8.0.2');");

            }
        }

        /// <summary>
        /// Seed data when the database is empty.
        /// </summary>
        public static async Task SeedDatabaseIfNeeded(IConfiguration configuration)
        {
            // Read the user to seed when the user table is empty from the appsettings
            // The hash can be manually calculated. If not supplied, use the built-in values
            // for which we keep the password a secret.
            string username = configuration.GetSection("AppSettings").GetValue<string>("DefaultUser") ?? "csadmin";
            string hash = configuration.GetSection("AppSettings").GetValue<string>("DefaultUserHash") ?? "c0ffb8e274979e7629b1859eb206fba010a90720f327791d88834ec4cd7a7340867b";

            using CentaurScoresDbContext context = new(configuration);
            int adminACLId = configuration.GetSection("AppSettings").GetValue<int>("AdminACLId");
            if (!context.Accounts.Any())
            {
                EntityEntry<AccountEntity> addedEntity = context.Accounts.Add(new AccountEntity { Username = username, SaltedPasswordHash = hash });
                if (adminACLId > 0)
                {
                    if (context.ACLs.Any(a => a.Id == adminACLId))
                    {
                        var acl = context.ACLs.Single(x => x.Id == adminACLId);
                        acl.Accounts.Add(addedEntity.Entity);
                    }
                    else
                    {
                        EntityEntry<AclEntity> aclEntity = context.ACLs.Add(new AclEntity { Id = adminACLId, Name = "Administrators" });
                        aclEntity.Entity.Accounts.Add(addedEntity.Entity);
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
