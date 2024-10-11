using CentaurScores.CompetitionLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.OpenApi.Validations;
using Mysqlx.Session;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            List<NewPersonalBestModel> allUpdatedPersonalBestsWithDuplicates = [];

            List<RulesetModel> rulesets = await GetRulesets();
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity participantList =
                await db.ParticipantLists
                    .AsNoTracking()
                    .Include(pp => pp.PersonalBestLists)
                        .ThenInclude(pbl => pbl.Entries)
                            .ThenInclude(pbl => pbl.Participant)
                    .Include(pp => pp.Competitions)
                        .ThenInclude(c => c.Matches)
                            .ThenInclude(m => m.Participants)
                    .FirstOrDefaultAsync(x => x.Id == memberListId)
                ?? throw new ArgumentException("Bad ID", nameof(memberListId));
            foreach (CompetitionEntity competition in participantList.Competitions.ToList())
            {
                foreach (MatchEntity match in competition.Matches.ToList())
                {
                    RulesetModel? ruleset = rulesets.FirstOrDefault(rs => rs.Code == match.RulesetCode);
                    if (null != ruleset)
                    {
                        List<NewPersonalBestModel> resultForMatch = CalculateUpdatedPersonalBestEntriesForMatch(participantList, competition, match, ruleset);
                        allUpdatedPersonalBestsWithDuplicates.AddRange(resultForMatch);
                    }
                }
            }

            List<NewPersonalBestModel> result = allUpdatedPersonalBestsWithDuplicates
                .GroupBy(x => x.ListId)
                .SelectMany(listGroup =>
                    listGroup
                        .GroupBy(x => $"{x.Participant.Id}-{x.Discipline}")
                        .Select(group => group.OrderByDescending(e => e.Score).First()))
                .ToList();

            return result;
        }

        private static List<NewPersonalBestModel> CalculateUpdatedPersonalBestEntriesForMatch(ParticipantListEntity participantList, CompetitionEntity competition, MatchEntity match, RulesetModel ruleset)
        {
            List<NewPersonalBestModel> result = [];

            List<GroupInfo> allClasses = JsonConvert.DeserializeObject<List<GroupInfo>>(match.GroupsJSON) ?? [];

            string competitionFormat = ruleset.CompetitionFormat.ToString();
            List<PersonalBestsListEntity> applicableBestLists = participantList.PersonalBestLists.Where(pbl => pbl.CompetitionFormat == competitionFormat).ToList();
            foreach (ParticipantEntity participant in match.Participants.Where(e => e.ParticipantListEntryId != null).ToList())
            {
                int score = participant.Score;

                string discipline = allClasses.FirstOrDefault(x => x.Code == participant.Group)?.Label ?? "Onbekend";
                ;
                // Find all existing PBL entries for the applicable lists with a lower score
                List<PersonalBestsListEntryEntity> entriesThatHaveBeenImprovedUpon = applicableBestLists
                    .SelectMany(x => x.Entries)
                    .Where(e => e.Score < score && e.Participant?.Id != null && e.Participant?.Id == participant.ParticipantListEntryId && e.Discipline == discipline)
                    .ToList();

                result.AddRange(entriesThatHaveBeenImprovedUpon.Select(
                    e => new NewPersonalBestModel()
                    {
                        ListId = e.List.Id ?? -1,
                        Achieved = DateFromMatchCode(match),
                        Competition = competition.ToMetadataModel(),
                        Score = score,
                        Discipline = e.Discipline,
                        Id = e.Id,
                        Match = match.ToModel(),
                        Participant = e.Participant.ToModel(),
                        Notes = $"{match.MatchCode} - {match.MatchName}",
                        PreviousScore = e.Score
                    }
                    ));

                List<PersonalBestsListEntity> listsThatParticipantIsNotInShouldBeNow =
                    applicableBestLists.Where(x => !(x.Entries.Any(e => e.Participant?.Id == participant.ParticipantListEntryId && e.Discipline == discipline)))
                    .ToList();

                result.AddRange(listsThatParticipantIsNotInShouldBeNow.Select(
                    e => new NewPersonalBestModel()
                    {
                        ListId = e.Id ?? -1,
                        Achieved = DateFromMatchCode(match),
                        Competition = competition.ToMetadataModel(),
                        Score = score,
                        Discipline = allClasses.FirstOrDefault(s => s.Code == participant.Group)?.Label ?? "Onbekend",
                        Id = null,
                        Match = match.ToModel(),
                        Participant = new() { Id = participant.ParticipantListEntryId, Name = participant.Name },
                        Notes = $"{match.MatchCode} - {match.MatchName}",
                    }
                    ));
            }

            return result;
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
