using CentaurScores.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB match participant. Do not confuse with "ParticipantListEntry" which is the actual member.
    /// </summary>
    public class ParticipantEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;
        /// <summary>
        /// Match for this record.
        /// </summary>
        public required MatchEntity Match { get; set; }
        /// <summary>
        /// Devicde ID for the device this record is on.
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;
        /// <summary>
        /// Lijn on that device.
        /// </summary>
        public string Lijn { get; set; } = string.Empty;
        /// <summary>
        /// Archer name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Code of the GroupInfo for the archer's discipline as represented in the match definition's Groups JSON.
        /// </summary>
        public string Group { get; set; } = string.Empty;
        /// <summary>
        /// Code of the GroupInfo for the archer's age group as represented in the match definition's Subgroups JSON.
        /// </summary>
        public string Subgroup { get; set; } = string.Empty;
        /// <summary>
        /// Target code.
        /// </summary>
        public string Target { get; set; } = string.Empty;
        /// <summary>
        /// Calculated score.
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// Contains the head to head match information in a JSON-encoded array per round.
        /// </summary>
        public string HeadToHeadJSON { get; set; } = "[]";
        /// <summary>
        /// All ends as JSON.
        /// </summary>
        public string EndsJSON { get; set; } = string.Empty;
        /// <summary>
        /// ID in the participant list for the competition for this archer. Same ID = same archer.
        /// </summary>
        public int? ParticipantListEntryId { get; set; } = null;

        public ParticipantModel ToModel(int activeRound)
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = (JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? []).Where(x => x.Round == activeRound).ToList(),
                Score = Score,
                HeadToHeadJSON = HeadToHeadJSON,
                Group = Group,
                Lijn = Lijn,
                Name = Name,
                Subgroup = Subgroup,
                Target = Target,
                DeviceID = DeviceID,
                ParticipantListEntryId = ParticipantListEntryId,
            };
        }

        public ParticipantModelV2 ToModelV2(GroupInfo[] groups, GroupInfo[] subgroups, GroupInfo[] targets, int activeRound)
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = (JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? []).Where(x => x.Round == activeRound).ToList(),
                Score = Score,
                HeadToHeadJSON = HeadToHeadJSON,
                Group = Group,
                Lijn = Lijn,
                Name = Name,
                Subgroup = Subgroup,
                Target = Target,
                DeviceID = DeviceID,
                ParticipantListEntryId = ParticipantListEntryId,
                GroupName = (groups ?? []).FirstOrDefault(e => e.Code == Group)?.Label,
                SubgroupName = (subgroups ?? []).FirstOrDefault(e => e.Code == Subgroup)?.Label,
                TargetName = (targets ?? []).FirstOrDefault(e => e.Code == Target)?.Label,
            };
        }

        public ParticipantModelV3 ToModelV3(GroupInfo[] groups, GroupInfo[] subgroups, GroupInfo[] targets, int activeRound)
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = (JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? []).Where(x => x.Round == activeRound).ToList(),
                Score = Score,
                HeadToHeadJSON = HeadToHeadJSON,
                Group = Group,
                Lijn = Lijn,
                Name = Name,
                Subgroup = Subgroup,
                Target = Target,
                DeviceID = DeviceID,
                ParticipantListEntryId = ParticipantListEntryId,
                GroupName = (groups ?? []).FirstOrDefault(e => e.Code == Group)?.Label,
                SubgroupName = (subgroups ?? []).FirstOrDefault(e => e.Code == Subgroup)?.Label,
                TargetName = (targets ?? []).FirstOrDefault(e => e.Code == Target)?.Label,
                H2HInfo = (JsonConvert.DeserializeObject<List<HeadToHeadInfoEntry>>(HeadToHeadJSON ?? "[]") ?? []),
            };
        }

        public void UpdateFromModel(int activeRound, ParticipantModel data)
        {
            Name = data.Name;
            Group = data.Group;
            Subgroup = data.Subgroup;
            Target = data.Target;
            Score = data.Score;
            // Not updating HeadToHeadJSON, this gets its own endpoint
            // When intentionally clearing this, set it to -1
            if (null != data.ParticipantListEntryId)
            {
                ParticipantListEntryId = data.ParticipantListEntryId;
            }
            if (!string.IsNullOrEmpty(data.Lijn))
            {
                Lijn = data.Lijn;
            }

            // Only update the ends for the currently active roud of the match. The other ends are not updated at all.
            List<EndModel> ends = JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? [];
            ends.RemoveAll(e => e.Round == activeRound);
            ends.AddRange(data.Ends.Select(e => { e.Round = activeRound; return e; }));
            EndsJSON = JsonConvert.SerializeObject(ends);
        }
    }
}
