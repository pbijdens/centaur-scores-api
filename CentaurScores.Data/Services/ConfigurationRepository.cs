
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CentaurScores.Services
{
    /// <inheritdoc/>
    public class ConfigurationRepository(IConfiguration configuration) : IConfigurationRepository
    {
        /// <inheritdoc/>
        public async Task<string?> GetValue(string key)
        {
            using var db = new CentaurScoresDbContext(configuration);
            CsSetting? entity = await db.Settings.FirstOrDefaultAsync(x => x.Name == CleanKey(key));
            return (entity == null) ? null : JsonConvert.DeserializeObject<string>(entity.JsonValue ?? "null");
        }

        /// <inheritdoc/>
        public async Task<bool> SetValue(string key, string? value)
        {
            using var db = new CentaurScoresDbContext(configuration);
            CsSetting? entity = await db.Settings.FirstOrDefaultAsync(x => x.Name == CleanKey(key));
            if (null == entity)
            {
                entity = new CsSetting { Name = CleanKey(key), JsonValue = JsonConvert.SerializeObject(value) };
                db.Add(entity);
            }
            else
            {
                entity.JsonValue = JsonConvert.SerializeObject(value);
            }
            await db.SaveChangesAsync();
            return true;
        }

        private string CleanKey(string key)
        {
            return "API-config-" + Regex.Replace($"{key}".ToLowerInvariant(), "[^a-z0-9]", ".");
        }
    }
}
