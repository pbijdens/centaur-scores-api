using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CentaurScores.Services
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IConfiguration configuration;

        public MatchRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private static void AutoFixParticipantModel(MatchModel match, ParticipantModel participant)
        {
            participant.Ends ??= new();
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
                    end.Arrows ??= new();
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
        }

        private static async Task<int> FetchActiveID(CentaurScoresDbContext db)
        {
            CsSetting? activeSetting = await db.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
            int? activeID = JsonConvert.DeserializeObject<int?>(activeSetting?.JsonValue ?? "-1");
            return activeID ?? -1;
        }

        private static async Task<MatchModel> GetMatchModelFromID(CentaurScoresDbContext db, int id, int activeID)
        {
            List<MatchEntity> entities = await db.Matches.AsNoTracking().Where(entity => entity.Id == id).ToListAsync();
            MatchModel? result = entities.Select(x => x.ToModel()).Select(x =>
            {
                x.IsActiveMatch = x.Id == activeID;
                return x;
            }).FirstOrDefault();
            if (null == result)
            {
                throw new InvalidOperationException($"GetMatch invoked for non-existing match {id}");
            }

            return result;
        }

        public async Task<MatchModel> ActivateMatch(int id, bool isActive)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                string jsonEncodedValue = isActive ? JsonConvert.SerializeObject(id) : "-1";
                CsSetting? setting = await db.Settings.FirstOrDefaultAsync(x => x.Name == CsSetting.ActiveMatchId);
                if (null == setting)
                {
                    setting = new CsSetting { Name = CsSetting.ActiveMatchId, JsonValue = jsonEncodedValue };
                    db.Add(setting);
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
            }
            return await GetMatch(id);
        }

        public async Task<MatchModel> CreateMatch(MatchModel match)
        {
            int createdObjectId = -1;
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                MatchEntity entity = new MatchEntity
                {
                    ArrowsPerEnd = match.ArrowsPerEnd,
                    AutoProgressAfterEachArrow = match.AutoProgressAfterEachArrow,
                    GroupsJSON = JsonConvert.SerializeObject(match.Groups),
                    LijnenJSON = JsonConvert.SerializeObject(match.Lijnen),
                    NumberOfEnds = match.NumberOfEnds,
                    Participants = new(),
                    ScoreValuesJson = JsonConvert.SerializeObject(match.ScoreValues),
                    SubgroupsJSON = JsonConvert.SerializeObject(match.Subgroups),
                    TargetsJSON = JsonConvert.SerializeObject(match.Targets),
                    MatchCode = match.MatchCode,
                    MatchName = match.MatchName,
                    RulesetCode = match.RulesetCode,
                };
                var result = db.Matches.Add(entity);
                await db.SaveChangesAsync();
                createdObjectId = result.Entity?.Id ?? -1;
            }
            return await GetMatch(createdObjectId);
        }

        public async Task<bool> DeleteMatch(int id)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                MatchEntity? matchEntity = await db.Matches.FirstOrDefaultAsync(entity => entity.Id == id);
                if (null != matchEntity)
                {
                    db.Matches.Remove(matchEntity);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<MatchModel>> FindMatches()
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                int activeID = await FetchActiveID(db);
                List<MatchEntity> entities = await db.Matches.AsNoTracking().OrderByDescending(entity => entity.MatchCode).ToListAsync();
                List<MatchModel> result = entities.Select(x => x.ToModel()).Select(x =>
                {
                    x.IsActiveMatch = x.Id == activeID;
                    return x;
                }).ToList();
                return result;
            }
        }


        public async Task<MatchModel> GetActiveMatch()
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                int activeID = await FetchActiveID(db);
                MatchModel result = await GetMatchModelFromID(db, activeID, activeID);
                return result;
            }
        }

        public async Task<MatchModel> GetMatch(int id)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                int activeID = await FetchActiveID(db);
                MatchModel result = await GetMatchModelFromID(db, id, activeID);
                return result;
            }
        }

        public async Task<List<ParticipantModel>> GetParticipantsForMatch(int id)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                List<ParticipantEntity> entities = await db.Participants.AsNoTracking().Where(x => x.Match.Id == id).OrderBy(entity => entity.Name).ToListAsync();
                List<ParticipantModel> result = entities.Select(x => x.ToModel()).ToList();
                return result;
            }
        }

        public async Task<List<ParticipantModel>> GetParticipantsForMatchByDeviceID(int id, string deviceID)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                int activeID = await FetchActiveID(db);
                MatchModel match = await GetMatchModelFromID(db, id, activeID);

                List<ParticipantEntity> entities = await db.Participants.AsNoTracking().Where(x => x.Match.Id == id && x.DeviceID == deviceID).OrderBy(entity => entity.Lijn).ToListAsync();
                List<ParticipantModel> participants = entities.Select(x => x.ToModel()).ToList();

                List<ParticipantModel> result = new();
                foreach (var lijn in match.Lijnen) // returns one participant for each "Lijn"
                {
                    ParticipantModel? participant = participants.FirstOrDefault(p => p.Lijn == lijn);
                    participant ??= new ParticipantModel { Lijn = lijn, Score = 0, Id = -1, Ends = new(), Name = string.Empty, Group = string.Empty, Subgroup = string.Empty, Target = string.Empty };
                    AutoFixParticipantModel(match, participant);
                    result.Add(participant);
                }
                return result;
            }
        }

        public async Task<MatchModel> UpdateMatch(int id, MatchModel match)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                MatchEntity? matchEntity = await db.Matches.Where(entity => entity.Id == id).FirstOrDefaultAsync();
                if (null == matchEntity)
                {
                    throw new InvalidOperationException($"UpdateMatch invoked for non-existing match {id}");
                }

                matchEntity.MatchCode = match.MatchCode;
                matchEntity.MatchName = match.MatchName;
                matchEntity.NumberOfEnds = match.NumberOfEnds;
                matchEntity.ArrowsPerEnd = match.ArrowsPerEnd;
                matchEntity.AutoProgressAfterEachArrow = match.AutoProgressAfterEachArrow;
                matchEntity.ScoreValuesJson = JsonConvert.SerializeObject(match.ScoreValues);
                matchEntity.GroupsJSON = JsonConvert.SerializeObject(match.Groups);
                matchEntity.SubgroupsJSON = JsonConvert.SerializeObject(match.Subgroups);
                matchEntity.TargetsJSON = JsonConvert.SerializeObject(match.Targets);
                matchEntity.LijnenJSON = JsonConvert.SerializeObject(match.Lijnen);
                matchEntity.RulesetCode = match.RulesetCode;

                await db.SaveChangesAsync();
            }
            return await GetMatch(id);
        }

        public async Task<int> UpdateParticipantsForMatch(int id, string deviceID, List<ParticipantModel> participants)
        {
            int result = 0;
            List<ParticipantModel> updateList = participants.ToList();

            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                int activeID = await FetchActiveID(db);
                if (activeID != id) throw new InvalidOperationException($"Updating data is only allowed for the currently active match.");

                MatchEntity? matchEntity = await db.Matches.Include(m => m.Participants).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (null == matchEntity) throw new InvalidOperationException($"Updating data for non-existent match {id} is not allowed.");

                var lijnen = JsonConvert.DeserializeObject<List<string>>(matchEntity.LijnenJSON) ?? new();
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
                        participantEntity.UpdateFromModel(updatedVersion);
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
                    ParticipantEntity participantEntity = new ParticipantEntity()
                    {
                        Match = matchEntity,
                        Lijn = remainingRecordWithName.Lijn,
                        DeviceID = deviceID,
                    };
                    AutoFixParticipantModel(matchEntity.ToModel(), remainingRecordWithName);
                    participantEntity.UpdateFromModel(remainingRecordWithName);
                    matchEntity.Participants.Add(participantEntity);
                }

                await db.SaveChangesAsync();

                return result;
            }
        }

        public async Task<bool> TransferParticipantForMatchToDevice(int id, int participantId, string targetDeviceID, string lijn)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                MatchEntity? matchEntity = await db.Matches.Where(entity => entity.Id == id).FirstOrDefaultAsync();
                if (null == matchEntity)
                {
                    throw new InvalidOperationException($"TransferParticipantForMatchToDevice invoked for non-existing match {id}");
                }

                ParticipantEntity? participant = db.Participants.Where(x => x.Id == participantId).FirstOrDefault();
                if (null == participant)
                {
                    throw new InvalidOperationException($"TransferParticipantForMatchToDevice invoked for non-existing participant {participantId}");
                }

                // If there is currently someone configured for this lijn, remove that record
                ParticipantEntity? existingParticipant = db.Participants.Where(x => x.Match.Id == id && x.DeviceID == targetDeviceID && x.Lijn == lijn).FirstOrDefault();
                if (null != existingParticipant && existingParticipant.Id != participantId) 
                {
                    db.Participants.Remove(existingParticipant);
                    await db.SaveChangesAsync();
                }

                // Update the transferred record
                participant.DeviceID = targetDeviceID;
                participant.Lijn = lijn;

                await db.SaveChangesAsync();

                return true;
            }
        }
    }
}
