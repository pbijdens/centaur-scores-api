using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CentaurScores.Services
{
    /// <summary>
    /// Offers personal best services
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="ruleServices"></param>
    public class PersonalBestService(IConfiguration configuration, IEnumerable<IRuleService> ruleServices) : IPersonalBestService
    {
        /// <inheritdoc/>
        public async Task<List<NewPersonalBestModel>> CalculateUpdatedRecords(int memberListId)
        {
            using var db = new CentaurScoresDbContext(configuration);

            List<NewPersonalBestModel> allUpdatedPersonalBestsWithDuplicates = [];

            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            ListConfigurationModel config = participantList.GetConfiguration();

            List<RulesetModel> rulesets = await GetRulesets();
            IQueryable<PersonalBestsListEntity> pbls = db.ParticipantLists.AsNoTracking().Include(pl => pl.PersonalBestLists).Where(pl => pl.Id == memberListId).SelectMany(pl => pl.PersonalBestLists);
            List<int?> pblIDs = pbls.Select(x => x.Id).Distinct().ToList().ToList();
            List<string> competitionFormats = pbls.Select(x => x.CompetitionFormat).Distinct().ToList();
            List<string> rulesetCodes = rulesets.Where(r => competitionFormats.Contains(r.CompetitionFormat)).Select(r => r.Code).ToList();

            Dictionary<string, string> rulesetToCf = [];
            rulesets.ForEach(r => rulesetToCf[r.Code] = r.CompetitionFormat);
            // only considering matcehes that have not been identified as head to head matches
            var registeredParticipants =
                    from matchParticipant in db.Participants.AsNoTracking().Include(p => p.Match).ThenInclude(m => m.Competition)
                        .Where(mp => mp.Match != null && mp.Match.Competition != null && mp.Match.Competition.ParticipantList != null && mp.Match.Competition.ParticipantList.Id == memberListId && (mp.Match.MatchFlags & MatchEntity.MatchFlagsHeadToHead) == 0)
                    from participantListEntry in db.ParticipantListEntries.AsNoTracking().Include(ple => ple.List)
                        .Where(pl => pblIDs.Contains(pl.List.Id))
                        .Where(pl => matchParticipant.ParticipantListEntryId == pl.Id)
                    select new
                    {
                        MatchId = matchParticipant.Match.Id,
                        matchParticipant.Match.MatchCode,
                        matchParticipant.Match.MatchName,
                        CompetitionId = matchParticipant.Match.Competition!.Id,
                        CompetitionName = matchParticipant.Match.Competition!.Name,
                        matchParticipant.Score,
                        matchParticipant.ParticipantListEntryId,
                        matchParticipant.Match.RulesetCode,
                        DisciplineCode = matchParticipant.Group,
                        participantListEntry.Name,
                    };


            // This is difficult to read, but the easy to read version had terrible performance
            var updatedPersonalBestRecords =
                 (from registeredParticipant in registeredParticipants
                  from personalBestListEntry in db.PersonalBestListEntries.AsNoTracking().Include(pble => pble.List).ThenInclude(pbl => pbl.ParticipantList)
                      .Where(pble => registeredParticipant.ParticipantListEntryId == pble.Participant.Id)
                      .DefaultIfEmpty()
                  select new
                  {
                      registeredParticipant.ParticipantListEntryId,
                      PersonalBestListEntry = personalBestListEntry,
                      registeredParticipant.Score,
                      PersonalBestScore = personalBestListEntry.Score,
                      PersonalBestDiscipline = personalBestListEntry.Discipline,
                      PersonalBestListId = personalBestListEntry.List.Id,
                      registeredParticipant.MatchCode,
                      registeredParticipant.MatchName,
                      registeredParticipant.CompetitionId,
                      registeredParticipant.CompetitionName,
                      registeredParticipant.DisciplineCode,
                      registeredParticipant.RulesetCode,
                      registeredParticipant.MatchId,
                      registeredParticipant.Name,
                  })
                 .Where(x => x.Score > x.PersonalBestScore)
                 .ToList()
                 .Where(x => (x.PersonalBestListEntry.List?.CompetitionFormat ?? "") == rulesetToCf[x.RulesetCode ?? ""])
                 .Select(x => new NewPersonalBestModel
                 {
                     Id = x.PersonalBestListEntry?.Id ?? -1,
                     Achieved = DateFromMatchCode(new MatchEntity { MatchCode = x.MatchCode, MatchName = x.MatchName }),
                     Competition = new CompetitionModel { Id = x.CompetitionId, Name = x.CompetitionName },
                     Discipline = x.PersonalBestDiscipline,
                     ListId = x.PersonalBestListId ?? -1,
                     Match = new MatchModel { Id = x.MatchId ?? -1, MatchName = x.MatchName, MatchCode = x.MatchCode },
                     Notes = $"Automatisch toegevoegd op basis van {x.MatchName} / {x.MatchCode}",
                     Participant = new ParticipantListMemberModel { Id = x.ParticipantListEntryId, Name = x.Name },
                     PreviousScore = x.PersonalBestScore,
                     Score = x.Score
                 })
                 .ToList();

            var newlyRequiredPersonalBestRecords =
                registeredParticipants.ToList().Select(registeredParticipant => new
                {
                    PD = registeredParticipant,
                    Discipline = config.Disciplines.FirstOrDefault(r => r.Code == registeredParticipant.DisciplineCode)?.Label ?? "",
                    ListID = db.PersonalBestLists.AsNoTracking().FirstOrDefault(x => x.CompetitionFormat == rulesetToCf[registeredParticipant.RulesetCode ?? ""])?.Id ?? -1
                })
                .Where(matchResult => matchResult.ListID >= 0)
                .Where(matchResult => !db.PersonalBestListEntries.Include(pble => pble.List).AsNoTracking().Any(pble => pble.Discipline == matchResult.Discipline && pble.List.CompetitionFormat == rulesetToCf[matchResult.PD.RulesetCode ?? ""] && pble.Participant.Id == matchResult.PD.ParticipantListEntryId))
                .Select(x => new NewPersonalBestModel
                {
                    Id = -1,
                    Achieved = DateFromMatchCode(new MatchEntity { MatchCode = x.PD.MatchCode, MatchName = x.PD.MatchName }),
                    Competition = new CompetitionModel { Id = x.PD.CompetitionId, Name = x.PD.CompetitionName },
                    Discipline = x.Discipline,
                    ListId = x.ListID,
                    Match = new MatchModel { Id = x.PD.MatchId ?? -1, MatchName = x.PD.MatchName, MatchCode = x.PD.MatchCode },
                    Notes = $"Automatisch toegevoegd op basis van {x.PD.MatchName} / {x.PD.MatchCode}",
                    Participant = new ParticipantListMemberModel { Id = x.PD.ParticipantListEntryId, Name = x.PD.Name },
                    PreviousScore = 0,
                    Score = x.PD.Score
                })
                .ToList();

            return updatedPersonalBestRecords
                .Concat(newlyRequiredPersonalBestRecords)
                .OrderBy(x => x.Participant.Name)
                    .ThenBy(x => x.Discipline)
                .ToList();
        }

        private static string DateFromMatchCode(MatchEntity match)
        {
            List<Match> regexMatches = Regex.Matches(match.MatchCode + " " + match.MatchName, "[0-9]{4}-[0-9]{2}-[0-9]{2}").ToList();
            foreach (Match regexMatch in regexMatches)
            {
                if (DateTimeOffset.TryParse(regexMatch.Value, out DateTimeOffset _))
                {
                    return regexMatch.Value;
                }
            }
            return DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");
        }

        /// <inheritdoc/>
        public async Task<PersonalBestListModel> CreatePersonalBestList(int memberListId, PersonalBestListModel list)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity entity = new() { CompetitionFormat = list.CompetitionFormat, ParticipantList = participantList };
            entity.UpdateFromModel(list);
            EntityEntry<PersonalBestsListEntity> createdEntityEntry = await db.PersonalBestLists.AddAsync(entity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return (await db.PersonalBestLists.SingleAsync(x => x.Id == createdObjectId)).ToModel();

        }

        /// <inheritdoc/>
        public async Task<PersonalBestListEntryModel> CreatePersonalBestListEntry(int memberListId, int personalBestListId, PersonalBestListEntryModel entry)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity list = await db.PersonalBestLists.SingleOrDefaultAsync(x => x.Id == personalBestListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListId));
            ParticipantListEntryEntity participant = await db.ParticipantListEntries.SingleOrDefaultAsync(x => x.Id == entry.Participant.Id) ?? throw new ArgumentException("Bad participant ID", nameof(entry));
            PersonalBestsListEntryEntity entity = new() { List = list, Participant = participant, Discipline = entry.Discipline };
            entity.UpdateFromModel(db, entry);
            EntityEntry<PersonalBestsListEntryEntity> createdEntityEntry = await db.PersonalBestListEntries.AddAsync(entity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return (await db.PersonalBestListEntries.SingleAsync(x => x.Id == createdObjectId)).ToModel();
        }

        /// <inheritdoc/>
        public async Task<int> DeletePersonalBestList(int memberListId, int personalBestListId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity list = await db.PersonalBestLists.SingleOrDefaultAsync(x => x.Id == personalBestListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListId));
            db.PersonalBestLists.Remove(list);
            await db.SaveChangesAsync();
            return 1;
        }

        /// <inheritdoc/>
        public async Task<int> DeletePersonalBestListEntry(int memberListId, int personalBestListId, int personalBestListEntryId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity list = await db.PersonalBestLists.SingleOrDefaultAsync(x => x.Id == personalBestListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListId));
            PersonalBestsListEntryEntity entry = await db.PersonalBestListEntries.SingleOrDefaultAsync(x => x.Id == personalBestListEntryId && x.List.Id == personalBestListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListEntryId));
            db.PersonalBestListEntries.Remove(entry);
            await db.SaveChangesAsync();
            return 1;
        }

        /// <inheritdoc/>
        public async Task<PersonalBestListModel> UpdatePersonalBestList(int memberListId, PersonalBestListModel listModel)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity list = await db.PersonalBestLists.SingleOrDefaultAsync(x => x.Id == listModel.Id && x.ParticipantList != null && x.ParticipantList.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(listModel));
            list.UpdateFromModel(listModel);
            await db.SaveChangesAsync();
            return list.ToModel();
        }

        /// <inheritdoc/>
        public async Task<PersonalBestListEntryModel> UpdatePersonalBestListEntry(int memberListId, int personalBestListId, PersonalBestListEntryModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity personalBestList = await db.PersonalBestLists.SingleOrDefaultAsync(x => x.Id == personalBestListId && x.ParticipantList != null && x.ParticipantList.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListId));
            PersonalBestsListEntryEntity entry = await db.PersonalBestListEntries.SingleOrDefaultAsync(x => x.Id == model.Id) ?? throw new ArgumentException("Bad ID", nameof(model));
            entry.UpdateFromModel(db, model);
            await db.SaveChangesAsync();
            return entry.ToModel();
        }

        /// <inheritdoc/>
        public async Task<List<PersonalBestListModel>> GetPersonalBestLists(int memberListId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            return await db.PersonalBestLists.OrderBy(x => x.Name).Where(e => e.ParticipantList != null && e.ParticipantList.Id == memberListId).Select(e => e.ToModel()).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<PersonalBestListModel> GetPersonalBestList(int memberListId, int personalBestListId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList = await db.ParticipantLists.SingleOrDefaultAsync(x => x.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            PersonalBestsListEntity entry = await db.PersonalBestLists.Include(x => x.Entries).ThenInclude(e => e.Participant).SingleOrDefaultAsync(x => x.Id == personalBestListId && x.ParticipantList != null && x.ParticipantList.Id == memberListId) ?? throw new ArgumentException("Bad ID", nameof(personalBestListId));

            PersonalBestListModel result = entry.ToModel();
            result.Entries.AddRange(entry.Entries.Select(e => e.ToModel()).OrderBy(x => x.Discipline).ThenByDescending(x => x.Score));
            return result;
        }

        /// <inheritdoc/>
        public async Task<PersonalBestListEntryModel> GetPersonalBestListrEntry(int memberListId, int personalBestListId, int memberId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            PersonalBestsListEntryEntity entity = await db.PersonalBestListEntries.Include(x => x.Participant).SingleOrDefaultAsync(x => x.Id == memberId) ?? throw new ArgumentException("Bad ID", nameof(memberId));
            return entity.ToModel();
        }

        private async Task<List<RulesetModel>> GetRulesets()
        {
            List<RulesetModel> result = [];
            foreach (var service in ruleServices)
            {
                result.AddRange(await service.GetSupportedRulesets());
            }
            return result;
        }
    }
}
