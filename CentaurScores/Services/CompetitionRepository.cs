using CentaurScores.CompetitionLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Configuration;

namespace CentaurScores.Services
{
    public class CompetitionRepository : ICompetitionRepository
    {
        private readonly IConfiguration configuration;
        private readonly IEnumerable<IRuleService> ruleServices;

        public CompetitionRepository(IConfiguration configuration, IEnumerable<IRuleService> ruleServices)
        {
            this.configuration = configuration;
            this.ruleServices = ruleServices;
        }

        public async Task<CompetitionResultModel?> CalculateCompetitionResult(int competitionId)
        {
            IRuleService? applicableRuleService = null;
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();
                CompetitionEntity? foundEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId);
                if (foundEntity != null)
                {
                    foreach (var service in ruleServices)
                    {
                        List<RulesetModel> supportedRulesets = await service.GetSupportedRulesets();
                        if (supportedRulesets.Any(x => x.GroupName == foundEntity.RulesetGroupName))
                        {
                            applicableRuleService = service;
                            break;
                        }
                    }
                }
            }

            // If any service could be found, use it.
            if (applicableRuleService != null)
            {
                return await applicableRuleService.CalculateCompetitionResult(competitionId);
            }
            return null;
        }

        public async Task<MatchResultModel?> CalculateSingleMatchResult(int matchId)
        {
            IRuleService? applicableRuleService = null;
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();
                MatchEntity? foundEntity = await db.Matches.FirstOrDefaultAsync(x => x.Id == matchId);
                if (foundEntity != null)
                {
                    foreach (var service in ruleServices)
                    {
                        List<RulesetModel> supportedRulesets = await service.GetSupportedRulesets();
                        if (supportedRulesets.Any(x => x.Code == foundEntity.RulesetCode))
                        {
                            applicableRuleService = service;
                            break;
                        }
                    }
                }
            }

            // If any service could be found, use it.
            if (applicableRuleService != null)
            {
                return await applicableRuleService.CalculateSingleMatchResult(matchId);
            }
            return null;
        }

        public async Task<CompetitionModel?> CreateCompetition(CompetitionModel model)
        {
            int createdObjectId = -1;
            using (var db = new CentaurScoresDbContext(configuration))
            {
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
                createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            }
            return await GetCompetition(createdObjectId);
        }

        public async Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model)
        {
            int createdObjectId = -1;
            using (var db = new CentaurScoresDbContext(configuration))
            {
                ParticipantListEntity newEntity = new();
                newEntity.UpdateFromModel(model);

                db.Database.EnsureCreated();

                EntityEntry<ParticipantListEntity> createdEntityEntry = await db.ParticipantLists.AddAsync(newEntity);
                await db.SaveChangesAsync();
                createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            }
            return await GetParticipantList(createdObjectId);
        }

        public async Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model)
        {
            int createdObjectId = -1;

            //ParticipantListEntity participantListEntity 
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (participantListEntity == null)
                {
                    throw new ArgumentException(nameof(listId), $"The participant list with that ID does not exist.");
                }

                ParticipantListEntryEntity participantListEntryEntity = new()
                {
                    List = participantListEntity,
                };
                participantListEntryEntity.UpdateFromModel(model);

                EntityEntry<ParticipantListEntryEntity> result = await db.ParticipantListEntries.AddAsync(participantListEntryEntity);
                await db.SaveChangesAsync();
                createdObjectId = result.Entity?.Id ?? -1;
                return await GetParticipantListMember(listId, createdObjectId);
            }
        }

        public async Task<int> DeleteCompetition(int competitionId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                CompetitionEntity? foundEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId);
                if (null != foundEntity)
                {
                    db.Competitions.Remove(foundEntity);
                    await db.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<int> DeleteParticipantList(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? foundEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != foundEntity)
                {
                    db.ParticipantLists.Remove(foundEntity);
                    await db.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<int> DeleteParticipantListMember(int listId, int memberId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? foundEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId);
                if (null != foundEntity)
                {
                    db.ParticipantListEntries.Remove(foundEntity);
                    await db.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<CompetitionModel?> GetCompetition(int competitionId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
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
                        result.Matches = foundEntity.Matches.Select(m => new MatchMetadataModel { 
                            Id = m.Id,
                            MatchCode = m.MatchCode,
                            MatchName = m.MatchName,
                            RulesetCode = m.RulesetCode,
                            IsActive = m.Id == activeId,
                        }).ToList();
                    }
                    return result;
                }
            }
            return null;
        }

        public async Task<List<CompetitionModel>> GetCompetitions()
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                var result = (await db.Competitions.Include(x => x.ParticipantList).OrderByDescending(x => x.StartDate).ThenBy(x => x.Name).ToListAsync())
                    .Select(x => x.ToMetadataModel())
                    .ToList();
                return result;
            }
        }

        public async Task<ParticipantListModel?> GetParticipantList(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    return participantListEntity.ToModel();
                }
            }
            return null;
        }

        public async Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.Id == memberId && x.List.Id == listId);
                if (null != participantListEntryEntity)
                {
                    return participantListEntryEntity.ToModel();
                }
            }
            return null;
        }

        public async Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.Include(x => x.Entries).FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    return participantListEntity.Entries.OrderBy(x => x.Name).ToList().Select(x => x.ToModel()).ToList();
                }
            }
            throw new ArgumentException(nameof(listId), $"A list with that ID does not exist");
        }

        public async Task<List<ParticipantListModel>> GetParticipantLists()
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();
                return await Task.FromResult(db.ParticipantLists.OrderBy(x => x.Name).ToList().Select(x => x.ToModel()).ToList());
            }
        }

        public async Task<List<RulesetModel>> GetRulesets()
        {
            List<RulesetModel> result = [];
            foreach (var service in ruleServices)
            {
                result.AddRange(await service.GetSupportedRulesets());
            }
            return result;
        }

        public async Task<CompetitionModel?> UpdateCompetition(int competitionId, CompetitionModel model)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                CompetitionEntity? foundEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId);
                if (null != foundEntity)
                {
                    foundEntity.UpdateMetadataFromModel(model);
                    if (model.ParticipantsList != null)
                    {
                        foundEntity.ParticipantList = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == model.ParticipantsList.Id);
                    }
                    await db.SaveChangesAsync();
                    CompetitionModel? result = await GetCompetition(competitionId);
                    return result;
                }
            }
            throw new ArgumentException(nameof(competitionId), $"A competition with that ID does not exist");
        }

        public async Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    participantListEntity.UpdateFromModel(model);
                    await db.SaveChangesAsync();
                    return await GetParticipantList(listId);
                }
            }
            throw new ArgumentException(nameof(listId), $"A list with that ID does not exist");
        }

        public async Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId);
                if (null != participantListEntryEntity)
                {
                    participantListEntryEntity.UpdateFromModel(model);
                    await db.SaveChangesAsync();
                    return await GetParticipantListMember(listId, memberId);
                }
            }
            throw new ArgumentException(nameof(memberId), $"A member with that ID does not exist in that list, or the list does not exist");
        }

        private static async Task<int> FetchActiveID(CentaurScoresDbContext db)
        {
            CsSetting? activeSetting = await db.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
            int? activeID = JsonConvert.DeserializeObject<int?>(activeSetting?.JsonValue ?? "-1");
            return activeID ?? -1;
        }
    }
}
