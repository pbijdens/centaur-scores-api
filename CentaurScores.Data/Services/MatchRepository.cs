using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CentaurScores.Services
{
    /// <inheritdoc/>
    /// <summary>Constructor</summary>
    public class MatchRepository(IConfiguration configuration) : IMatchRepository
    {
        /// <inheritdoc/>
        public static void AutoFixParticipantModel(MatchModel match, ParticipantModel participant)
        {
            participant.Ends ??= [];
            if (participant.Ends.Count != match.NumberOfEnds)
            {
                participant.Ends = participant.Ends.Take(match.NumberOfEnds).ToList();
                while (participant.Ends.Count < match.NumberOfEnds)
                {
                    participant.Ends.Add(new EndModel());
                }

                participant.Score = 0;
                foreach (EndModel end in participant.Ends)
                {
                    end.Arrows ??= [];
                    if (end.Arrows.Count != match.ArrowsPerEnd)
                    {
                        end.Arrows = end.Arrows.Take(match.ArrowsPerEnd).ToList();
                        while (end.Arrows.Count < match.ArrowsPerEnd)
                        {
                            end.Arrows.Add(null);
                        }
                        end.Score = null;
                        end.Arrows.ForEach(a =>
                        {
                            if (a.HasValue)
                            {
                                end.Score ??= 0;
                                end.Score += a.Value;
                            }
                        });
                        participant.Score += end.Score ?? 0;
                    }
                }
            }
            else
            {
                participant.Ends.ForEach(e => e.Score = e.Arrows.Sum(a => a ?? 0));
                participant.Score = participant.Ends.Sum(e => e.Arrows.Sum(a => a ?? 0));
            }
        }

        private static async Task<int> FetchActiveID(CentaurScoresDbContext db)
        {
            CsSetting? activeSetting = await db.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
            int? activeID = JsonConvert.DeserializeObject<int?>(activeSetting?.JsonValue ?? "-1");
            return activeID ?? -1;
        }

        private static async Task<MatchModel> GetMatchModelFromID(CentaurScoresDbContext db, int id, int activeID)
        {
            List<MatchEntity> entities = await db.GetMatchEntitiesUntracked().Where(entity => entity.Id == id).ToListAsync();
            MatchModel? result = entities.Select(x => x.ToModel()).Select(x =>
            {
                x.IsActiveMatch = x.Id == activeID;
                return x;
            }).FirstOrDefault();
            if (null == result)
            {
                throw new ArgumentException($"GetMatch invoked for non-existing match {id}", nameof(id));
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<MatchModel> ActivateMatch(int id, bool isActive)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            string jsonEncodedValue = isActive ? JsonConvert.SerializeObject(id) : "-1";
            CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
            if (null == setting)
            {
                setting = new CsSetting { Name = CsSetting.ActiveMatchId, JsonValue = jsonEncodedValue };
                db.Settings.Add(setting);
                await db.SaveChangesAsync();
            }
            else
            {
                // only allow deactivating the currently active match
                if (isActive || setting.JsonValue == JsonConvert.SerializeObject(id))
                {
                    setting.JsonValue = jsonEncodedValue;
                    await db.SaveChangesAsync();
                }
            }
            return await GetMatch(id);
        }

        /// <inheritdoc/>
        public async Task<MatchModel> CreateMatch(MatchModel match)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity entity = new()
            {
                ArrowsPerEnd = match.ArrowsPerEnd,
                AutoProgressAfterEachArrow = match.AutoProgressAfterEachArrow,
                LijnenJSON = JsonConvert.SerializeObject(match.Lijnen),
                NumberOfEnds = match.NumberOfEnds,
                Participants = [],
                MatchCode = match.MatchCode,
                MatchName = match.MatchName,
                RulesetCode = match.RulesetCode,
                MatchFlags = match.MatchFlags,
                ActiveRound = match.ActiveRound,
                NumberOfRounds = match.NumberOfRounds,

                // No longer used, moved to participant list configuration instead,
                // global for all competitions in a list
                GroupsJSON = "[]",
                ScoreValuesJson = "{}",
                SubgroupsJSON = "[]",
                TargetsJSON = "[]",
            };
            if (match.Competition != null && match.Competition.Id > 0)
            {
                entity.Competition = await db.Competitions.FirstOrDefaultAsync(x => x.Id == match.Competition.Id);
            }
            var result = db.Matches.Add(entity);
            await db.SaveChangesAsync();

            int createdObjectId = result.Entity?.Id ?? -1;
            return await GetMatch(createdObjectId);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMatch(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity? matchEntity = await db.Matches.FirstOrDefaultAsync(entity => entity.Id == id);
            if (null != matchEntity)
            {
                db.Matches.Remove(matchEntity);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task<List<MatchModel>> FindMatches(int? listId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeID = await FetchActiveID(db);
            List<MatchEntity> entities = await db.GetMatchEntitiesUntracked()
                .Include(x => x.Competition)
                .Where(entity => entity.Competition != null && (listId != null && listId >= 0 && (entity.Competition.ParticipantList != null && entity.Competition.ParticipantList.Id == listId)))
                .OrderBy(entity => entity.Competition!.Name)
                .OrderByDescending(entity => entity.MatchCode)
                .ToListAsync();
            List<MatchModel> result = entities.Select(x => x.ToModel()).Select(x =>
            {
                x.IsActiveMatch = x.Id == activeID;
                return x;
            }).ToList();
            return result;
        }


        /// <inheritdoc/>
        public async Task<MatchModel?> GetActiveMatch()
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeID = await FetchActiveID(db);
            if (activeID >= 0)
            {
                MatchModel result = await GetMatchModelFromID(db, activeID, activeID);
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<MatchModel> GetMatch(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeID = await FetchActiveID(db);
            MatchModel result = await GetMatchModelFromID(db, id, activeID);
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<ParticipantModelV3>> GetParticipantsForMatch(int id, int? round = null)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity match = await db.GetMatchEntitiesUntracked().SingleOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Bad match ID", nameof(id));
            MatchModel matchModel = match.ToModel();

            List<ParticipantEntity> entities;
            if ((match.MatchFlags & MatchEntity.MatchFlagsHeadToHead) != 0x0)
            {
                // Head to head match: return only participants that take part in the current round, all other participants are dead to us.
                entities = await db.Participants.AsNoTracking().Where(x => x.Match.Id == id).OrderBy(entity => entity.Name).ToListAsync();
                entities = entities.Where(e =>
                {
                    var h2h = JsonConvert.DeserializeObject<List<HeadToHeadInfoEntry>>(e.HeadToHeadJSON ?? "[]");
                    bool result = h2h != null && h2h.Count >= (round.HasValue ? round : match.ActiveRound);
                    return result;
                }).ToList();
            }
            else
            {
                entities = await db.Participants.AsNoTracking().Where(x => x.Match.Id == id).OrderBy(entity => entity.Name).ToListAsync();
            }

            List<ParticipantModelV3> result = entities.Select(x => x.ToModelV3(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, match.ActiveRound)).ToList();
            result.ForEach(x =>
            {
                AutoFixParticipantModel(match.ToModel(), x);
            });
            if ((match.MatchFlags & MatchEntity.MatchFlagsHeadToHead) == MatchEntity.MatchFlagsHeadToHead)
            {
                result = [.. result
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.H2HInfo.Count >= match.ActiveRound ? x.H2HInfo[Math.Max(0, match.ActiveRound - 1)].InitialPosition : -1)
                    .ThenBy(x => x.Name)];
            }
            return [.. result.OfType<ParticipantModelV3>()];
        }

        /// <inheritdoc/>
        public async Task<List<ParticipantModel>> GetParticipantsForMatchByDeviceID(int id, string deviceID)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeID = await FetchActiveID(db);
            MatchModel match = await GetMatchModelFromID(db, id, activeID);

            List<ParticipantEntity> entities = await db.Participants.AsNoTracking().Where(x => x.Match.Id == id && x.DeviceID == deviceID).OrderBy(entity => entity.Lijn).ToListAsync();
            List<ParticipantModel> participants = entities.Select(x => x.ToModel(match.ActiveRound)).ToList();

            List<ParticipantModel> result = [];
            foreach (var lijn in match.Lijnen) // returns one participant for each "Lijn"
            {
                ParticipantModel? participant = participants.FirstOrDefault(p => p.Lijn == lijn);
                participant ??= new ParticipantModel { Lijn = lijn, Score = 0, Id = -1, Ends = [], Name = string.Empty, Group = string.Empty, Subgroup = string.Empty, Target = string.Empty };
                AutoFixParticipantModel(match, participant);
                result.Add(participant);
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<MatchModel> UpdateMatch(int id, MatchModel match)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                MatchEntity matchEntity = await db.Matches.Where(entity => entity.Id == id).FirstOrDefaultAsync() ?? throw new ArgumentException($"UpdateMatch invoked for non-existing match {id}", nameof(id));
                matchEntity.MatchCode = match.MatchCode;
                matchEntity.MatchName = match.MatchName;
                matchEntity.NumberOfEnds = match.NumberOfEnds;
                matchEntity.ArrowsPerEnd = match.ArrowsPerEnd;
                matchEntity.AutoProgressAfterEachArrow = match.AutoProgressAfterEachArrow;
                matchEntity.LijnenJSON = JsonConvert.SerializeObject(match.Lijnen);
                matchEntity.RulesetCode = match.RulesetCode;
                matchEntity.MatchFlags = match.MatchFlags;
                matchEntity.ActiveRound = match.ActiveRound;
                matchEntity.NumberOfRounds = match.NumberOfRounds;

                // TODO: These are no longer supported and should be removed in due time
                if (matchEntity.GroupsJSON != "[]")
                {
                    matchEntity.ScoreValuesJson = JsonConvert.SerializeObject(match.ScoreValues);
                    matchEntity.GroupsJSON = JsonConvert.SerializeObject(match.Groups);
                    matchEntity.SubgroupsJSON = JsonConvert.SerializeObject(match.Subgroups);
                    matchEntity.TargetsJSON = JsonConvert.SerializeObject(match.Targets);
                }

                await db.SaveChangesAsync();
            }
            return await GetMatch(id);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateParticipantsForMatch(int id, string deviceID, List<ParticipantModel> participants)
        {
            int result = 0;
            List<ParticipantModel> updateList = [.. participants];

            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            int activeID = await FetchActiveID(db);
            if (activeID != id) throw new InvalidOperationException($"Updating data is only allowed for the currently active match.");

            MatchEntity? matchEntity = await db.GetMatchEntities().Include(m => m.Participants).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (null == matchEntity) throw new InvalidOperationException($"Updating data for non-existent match {id} is not allowed.");

            var lijnen = JsonConvert.DeserializeObject<List<string>>(matchEntity.LijnenJSON) ?? [];
            updateList.RemoveAll(participant => !lijnen.Any(lijn => lijn == participant.Lijn)); // disregard input that should not exist                

            // We'll overwrite any existing records for the LIJN and delete records for a LIJN that's not in the match anymore
            List<ParticipantEntity> currentParticipantEntitiesForMatch = matchEntity.Participants.Where(p => p.DeviceID == deviceID).ToList();
            foreach (var participantEntity in currentParticipantEntitiesForMatch)
            {
                ParticipantModel? updatedVersion = updateList.FirstOrDefault(entity => entity.Lijn == participantEntity.Lijn);
                if (null != updatedVersion)
                {
                    result++;
                    AutoFixParticipantModel(matchEntity.ToModel(), updatedVersion);
                    participantEntity.UpdateFromModel(matchEntity.ActiveRound, updatedVersion);
                    updateList.Remove(updatedVersion);
                }
                else
                {
                    result++;
                    db.Participants.Remove(participantEntity);
                }
            }

            // We'll add new records for those records that have both a valid LIJN and a valid NAME
            updateList.RemoveAll(participant => string.IsNullOrEmpty(participant.Name)); // do not remove these records, deal with them later
            foreach (ParticipantModel remainingRecordWithName in updateList)
            {
                result++;
                ParticipantEntity participantEntity = new()
                {
                    Match = matchEntity,
                    Lijn = remainingRecordWithName.Lijn,
                    DeviceID = deviceID,
                };
                AutoFixParticipantModel(matchEntity.ToModel(), remainingRecordWithName);
                participantEntity.UpdateFromModel(matchEntity.ActiveRound, remainingRecordWithName);
                matchEntity.Participants.Add(participantEntity);
            }

            await db.SaveChangesAsync();

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> TransferParticipantForMatchToDevice(int id, int participantId, string targetDeviceID, string lijn)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity matchEntity = await db.GetMatchEntities().Where(entity => entity.Id == id).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid match ID", nameof(id));

            ParticipantEntity? participant = db.Participants.Where(x => x.Id == participantId).FirstOrDefault() ?? throw new ArgumentException("Invalid participant ID", nameof(participantId));

            if (!string.IsNullOrWhiteSpace(participant.DeviceID) && participant.DeviceID != Guid.Empty.ToString())
            {
                await RequestDeviceSynchronization(db, participant.DeviceID);
                await db.SaveChangesAsync();
            }

            // If there is currently someone configured for this lijn, change the device ID so it is no longer 
            ParticipantEntity? existingParticipant = db.Participants.Where(x => x.Match.Id == id && x.DeviceID == targetDeviceID && x.Lijn == lijn).FirstOrDefault();
            if (null != existingParticipant && existingParticipant.Id != participantId)
            {
                await RequestDeviceSynchronization(db, existingParticipant.DeviceID);
                existingParticipant.DeviceID = string.Empty;
                await db.SaveChangesAsync();
            }

            // Update the transferred record
            participant.DeviceID = targetDeviceID;
            participant.Lijn = lijn;

            await RequestDeviceSynchronization(db, targetDeviceID);

            await db.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<ParticipantModel> GetParticipantForMatch(int id, int participantId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchModel matchModel = (await db.GetMatchEntitiesUntracked().Where(entity => entity.Id == id).FirstOrDefaultAsync())?.ToModel() ?? throw new ArgumentException("Invalid match ID", nameof(id));
            ParticipantEntity participantEntity = await db.Participants.AsNoTracking().Where(x => x.Id == participantId).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid participant ID", nameof(participantId));

            ParticipantModel model = participantEntity.ToModel(matchModel.ActiveRound);
            AutoFixParticipantModel(matchModel, model);

            return model;
        }

        /// <inheritdoc/>
        public async Task<ParticipantModel> UpdateParticipantForMatch(int id, int participantId, ParticipantModel participant)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity matchEntity = await db.GetMatchEntitiesUntracked().FirstOrDefaultAsync(entity => entity.Id == id) ?? throw new ArgumentException("Invalid match ID", nameof(id));
            MatchModel matchModel = matchEntity.ToModel();
            ParticipantEntity participantEntity = await db.Participants.Where(x => x.Id == participantId).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid participant ID", nameof(participantId));

            AutoFixParticipantModel(matchModel, participant);
            participantEntity.UpdateFromModel(matchModel.ActiveRound, participant);

            await RequestDeviceSynchronization(db, participant.DeviceID);

            await db.SaveChangesAsync();

            return await GetParticipantForMatch(id, participantId);
        }

        /// <inheritdoc/>
        public async Task<int> DeleteParticipantForMatch(int id, int participantId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantEntity? foundEntity = await db.Participants.FirstOrDefaultAsync(x => x.Id == participantId);
            if (null != foundEntity)
            {
                if (!string.IsNullOrWhiteSpace(foundEntity.DeviceID) && foundEntity.DeviceID != Guid.Empty.ToString())
                {
                    await RequestDeviceSynchronization(db, foundEntity.DeviceID);
                }
                db.Participants.Remove(foundEntity);
                await db.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        /// <inheritdoc/>
        public async Task<ParticipantModel> CreateParticipantForMatch(int id, ParticipantModel participantModel)
        {
            using CentaurScoresDbContext db = new(configuration);
            db.Database.EnsureCreated();

            MatchEntity matchEntity = await db.Matches.Where(entity => entity.Id == id).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid match ID", nameof(id));

            bool isHeadToHead = (matchEntity.MatchFlags & MatchEntity.MatchFlagsHeadToHead) != 0;

            ParticipantEntity participantEntity = new()
            {
                Match = matchEntity,
                Lijn = String.Empty,
                DeviceID = String.Empty,
            };
            AutoFixParticipantModel(matchEntity.ToModel(), participantModel);
            participantEntity.UpdateFromModel(matchEntity.ActiveRound, participantModel);
            if (isHeadToHead) // In a head-to-head match add some metadata to the participant
            {
                participantEntity.HeadToHeadJSON = JsonConvert.SerializeObject(new HeadToHeadInfoEntry[] {new HeadToHeadInfoEntry{
                    InitialPosition = await CalculateNextInitialPositionFor(db, matchEntity, participantModel.Group),
                    Bracket = 0,
                    IsSetScored = true,
                    IsWinner = false,
                    OpponentId = -1,
                    Position = -1
                }});
            }
            EntityEntry<ParticipantEntity> newEntity = db.Participants.Add(participantEntity);

            await db.SaveChangesAsync();

            return await GetParticipantForMatch(id, newEntity.Entity.Id ?? -1);
        }

        /// <inheritdoc/>
        public async Task MoveParticipant(int id, int participantId, int direction)
        {
            using CentaurScoresDbContext db = new(configuration);
            MatchEntity matchEntity = await db.Matches.Where(entity => entity.Id == id).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid match ID", nameof(id));
            MatchModel matchModel = matchEntity.ToModel();

            bool isHeadToHead = (matchEntity.MatchFlags & MatchEntity.MatchFlagsHeadToHead) != 0;

            List<ParticipantEntity> participants = await db.Participants.Where(p => p.Match.Id == id).ToListAsync();
            List<ParticipantModelV3> models = participants.Select(p => p.ToModelV3(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, matchEntity.ActiveRound)).OrderBy(p => p.H2HInfo.FirstOrDefault()?.InitialPosition).ToList();
            int index = models.FindIndex(x => x.Id == participantId);

            // Check if the operation would make sense, otherwise just ignore it
            if (index == -1 || index + direction < 0 || index + direction >= models.Count)
            {
                return;
            }

            ParticipantEntity p1 = participants.FirstOrDefault(x => x.Id == models[index].Id) ?? throw new InvalidOperationException();
            ParticipantEntity p2 = participants.FirstOrDefault(x => x.Id == models[index + direction].Id) ?? throw new InvalidOperationException();

            HeadToHeadInfoEntry[] l1 = JsonConvert.DeserializeObject<HeadToHeadInfoEntry[]>(p1.HeadToHeadJSON ?? "[]") ?? throw new InvalidOperationException();
            HeadToHeadInfoEntry[] l2 = JsonConvert.DeserializeObject<HeadToHeadInfoEntry[]>(p2.HeadToHeadJSON ?? "[]") ?? throw new InvalidOperationException();

            if (l1.Length == 0 || l2.Length == 0)
            {
                throw new InvalidOperationException();
            }

            (l2[0].InitialPosition, l1[0].InitialPosition) = (l1[0].InitialPosition, l2[0].InitialPosition);

            p1.HeadToHeadJSON = JsonConvert.SerializeObject(l1);
            p2.HeadToHeadJSON = JsonConvert.SerializeObject(l2);

            await db.SaveChangesAsync();
        }

        private async Task<int> CalculateNextInitialPositionFor(CentaurScoresDbContext db, MatchEntity matchEntity, string group)
        {
            List<string> jsons = await db.Participants.Where(p => p.Match.Id == matchEntity.Id).Select(p => p.HeadToHeadJSON).ToListAsync();
            if (jsons.Count() == 0) return 1;
            int currentMaximum = jsons
                .Select(x => JsonConvert.DeserializeObject<HeadToHeadInfoEntry[]>(x))
                .Where(x => x != null)
                .Select(x => x?.FirstOrDefault())
                .Select(x => x?.InitialPosition ?? 0)
                .ToList()
                .Max();
            return currentMaximum + 1;
        }

        /// <inheritdoc/>
        public async Task ClearRemotelyChangedFlag(int matchId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            MatchEntity match = await db.Matches.Where(entity => entity.Id == matchId).FirstOrDefaultAsync() ?? throw new ArgumentException("Invalid match ID", nameof(matchId));
            match.ChangedRemotely = false;

            await db.SaveChangesAsync();
        }

        private static async Task RequestDeviceSynchronization(CentaurScoresDbContext db, string deviceId)
        {
            // TODO: Might want to use some form of cross-process synchronization here because multiple clients might requets this at the same time.
            CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == CsSetting.DevicesNeedingForcedSync);
            if (null == setting)
            {
                setting = new CsSetting { Name = CsSetting.DevicesNeedingForcedSync, JsonValue = JsonConvert.SerializeObject(new string[] { deviceId }) };
                db.Settings.Add(setting);
            }
            else
            {
                List<string> values = JsonConvert.DeserializeObject<List<string>>(setting.JsonValue ?? "[]") ?? [];
                if (!values.Contains(deviceId)) values.Add(deviceId);
                setting.JsonValue = JsonConvert.SerializeObject(values);
            }
        }

        /// <inheritdoc/>
        public async Task ClearDeviceSynchronization(string deviceId)
        {
            // TODO: Might want to use some form of cross-process synchronization here because multiple clients might requets this at the same time.
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == CsSetting.DevicesNeedingForcedSync);
            if (null != setting)
            {
                List<string> values = JsonConvert.DeserializeObject<List<string>>(setting.JsonValue ?? "[]") ?? [];
                if (values.Contains(deviceId)) values.RemoveAll(x => x == deviceId);
                setting.JsonValue = JsonConvert.SerializeObject(values);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckDeviceSynchronization(string deviceId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            CsSetting? setting = await db.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == CsSetting.DevicesNeedingForcedSync);
            if (null != setting)
            {
                List<string> values = JsonConvert.DeserializeObject<List<string>>(setting.JsonValue ?? "[]") ?? [];
                return values.Contains(deviceId);
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task<string?> GetMatchUiSetting(int id, string name)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            string cleanName = Regex.Replace(name, "[^0-9a-z]", "_", RegexOptions.IgnoreCase);

            CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == $"CsUi-Match{id}-{cleanName}");
            return JsonConvert.DeserializeObject<string>(setting?.JsonValue ?? "") ?? "null";
        }


        /// <inheritdoc/>
        public async Task<string?> UpdateMatchUiSetting(int id, string name, string value)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            string cleanName = Regex.Replace(name, "[^0-9a-z]", "_", RegexOptions.IgnoreCase);
            string jsonEncodedValue = JsonConvert.SerializeObject(value);

            CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == $"CsUi-Match{id}-{cleanName}");
            if (null == setting)
            {
                setting = new CsSetting { Name = $"CsUi-Match{id}-{cleanName}", JsonValue = jsonEncodedValue };
                db.Settings.Add(setting);
                await db.SaveChangesAsync();
            }
            else
            {
                setting.JsonValue = jsonEncodedValue;
                await db.SaveChangesAsync();
            }
            return value;
        }

        public async Task UpdateParticipantHeadToHeadInfo(ParticipantModel participant, HeadToHeadInfoEntry[] info)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantEntity? foundEntity = await db.Participants.FirstOrDefaultAsync(x => x.Id == participant.Id);
            if (null != foundEntity)
            {
                participant.HeadToHeadJSON = JsonConvert.SerializeObject(info);
                foundEntity.HeadToHeadJSON = JsonConvert.SerializeObject(info);
                await db.SaveChangesAsync();
            }
        }
    }
}
