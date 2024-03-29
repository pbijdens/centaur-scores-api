﻿using CentaurScores.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CentaurScores.Persistence
{
    public class ParticipantEntity
    {
        public int? Id { get; set; } = null;
        public required MatchEntity Match { get; set; }
        public string DeviceID { get; set; } = string.Empty;
        public string Lijn { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int Score { get; set; }
        public string EndsJSON { get; set; } = string.Empty;

        internal ParticipantModel ToModel()
        {
            return new()
            {
                Id = Id ?? -1,
                Ends = JsonConvert.DeserializeObject<List<EndModel>>(EndsJSON) ?? new(),
                Score = Score,
                Group = Group,
                Lijn = Lijn,
                Name = Name,
                Subgroup = Subgroup,
                Target = Target,
                DeviceID = DeviceID,
            };
        }

        internal void UpdateFromModel(ParticipantModel data)
        {
            Name = data.Name;
            Group = data.Group;
            Subgroup = data.Subgroup;
            Target = data.Target;
            Score = data.Score;
            if (!string.IsNullOrEmpty(data.Lijn))
            {
                Lijn = data.Lijn;
            }
            EndsJSON = JsonConvert.SerializeObject(data.Ends);
        }
    }
}
