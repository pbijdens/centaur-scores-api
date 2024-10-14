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
        /// All ends as JSON.
        /// </summary>
        public string EndsJSON { get; set; } = string.Empty;
        /// <summary>
        /// ID in the participant list for the competition for this archer. Same ID = same archer.
        /// </summary>
        public int? ParticipantListEntryId { get; set; } = null;

        internal ParticipantModel ToModel()
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? [],
                Score = Score,
                Group = Group,
                Lijn = Lijn,
                Name = Name,
                Subgroup = Subgroup,
                Target = Target,
                DeviceID = DeviceID,
                ParticipantListEntryId = ParticipantListEntryId,
            };
        }

        internal ParticipantModelV2 ToModelV2(GroupInfo[] groups, GroupInfo[] subgroups, GroupInfo[] targets)
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? [],
                Score = Score,
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

        internal void UpdateFromModel(ParticipantModel data)
        {
            Name = data.Name;
            Group = data.Group;
            Subgroup = data.Subgroup;
            Target = data.Target;
            Score = data.Score;
            // When intentionally clearing this, set it to -1
            if (null != data.ParticipantListEntryId)
            {
                ParticipantListEntryId = data.ParticipantListEntryId;
            }
            if (!string.IsNullOrEmpty(data.Lijn))
            {
                Lijn = data.Lijn;
            }
            EndsJSON = JsonConvert.SerializeObject(data.Ends);
        }
    }
}
