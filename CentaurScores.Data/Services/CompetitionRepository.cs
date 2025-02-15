using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CentaurScores.Services
{
    /// <summary>
    /// Competition-related logic 
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class CompetitionRepository(IConfiguration configuration) : ICompetitionRepository
    {
        /// <inheritdoc/>
        public async Task<CompetitionModel?> CreateCompetition(CompetitionModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            CompetitionEntity newEntity = new();
            newEntity.UpdateMetadataFromModel(model);
            if (model.ParticipantsList != null)
            {
                newEntity.ParticipantList = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == model.ParticipantsList.Id);
            }
            // We do not take matches as input, that relationship is built the other way

            EntityEntry<CompetitionEntity> createdEntityEntry = await db.Competitions.AddAsync(newEntity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return await GetCompetition(createdObjectId);
        }

        /// <inheritdoc/>
        public async Task<int> DeleteCompetition(int competitionId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            CompetitionEntity? foundEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId);
            if (null != foundEntity)
            {
                db.Competitions.Remove(foundEntity);
                await db.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        /// <inheritdoc/>
        public async Task<CompetitionModel?> GetCompetition(int competitionId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeId = await FetchActiveID(db);

            CompetitionEntity? foundEntity = await db.Competitions.Include(x => x.ParticipantList).Include(x => x.Matches).FirstOrDefaultAsync(x => x.Id == competitionId);
            if (null != foundEntity)
            {
                CompetitionModel result = foundEntity.ToMetadataModel();
                if (foundEntity.ParticipantList != null)
                {
                    result.ParticipantsList = new()
                    {
                        Id = foundEntity.ParticipantList.Id,
                        Name = foundEntity.ParticipantList.Name,
                    };
                }
                if (foundEntity.Matches != null)
                {
                    result.Matches = foundEntity.Matches.Select(m => new MatchMetadataModel
                    {
                        Id = m.Id,
                        MatchCode = m.MatchCode,
                        MatchName = m.MatchName,
                        RulesetCode = m.RulesetCode,
                        IsActive = m.Id == activeId,
                    }).ToList();
                }
                return result;
            }
            else
            {
                throw new ArgumentException("Not found", nameof(competitionId));
            }
        }

        /// <inheritdoc/>
        public async Task<List<CompetitionModel>> GetCompetitions(int? listId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            var result = (await db.Competitions
                    .Include(x => x.ParticipantList)
                    .Where(x => listId == null || (x.ParticipantList != null && x.ParticipantList.Id == listId))
                    .OrderByDescending(x => x.StartDate)
                    .ThenBy(x => x.Name)
                    .ToListAsync())
                .Select(x => x.ToMetadataModel())
                .ToList();
            return result;
        }

        /// <inheritdoc/>
        public async Task<CompetitionModel?> UpdateCompetition(int competitionId, CompetitionModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            CompetitionEntity foundEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId) ?? throw new ArgumentException($"A competition with that ID does not exist", nameof(competitionId));
            foundEntity.UpdateMetadataFromModel(model);
            if (model.ParticipantsList != null)
            {
                foundEntity.ParticipantList = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == model.ParticipantsList.Id);
            }
            await db.SaveChangesAsync();
            CompetitionModel? result = await GetCompetition(competitionId);
            return result;
        }

        private static async Task<int> FetchActiveID(CentaurScoresDbContext db)
        {
            CsSetting? activeSetting = await db.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
            int? activeID = JsonConvert.DeserializeObject<int?>(activeSetting?.JsonValue ?? "-1");
            return activeID ?? -1;
        }
    }
}
