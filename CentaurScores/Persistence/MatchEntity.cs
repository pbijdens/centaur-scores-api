using CentaurScores.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CentaurScores.Persistence
{
    public class MatchEntity
    {
        /// <summary>Key</summary>
        public int? Id { get; set; }

        public string MatchCode { get; set; } = string.Empty;
        public string MatchName { get; set; } = string.Empty;
        public int NumberOfEnds { get; set; } = -1;
        public int ArrowsPerEnd { get; set; } = -1;
        public bool AutoProgressAfterEachArrow { get; set; } = false;
        public string ScoreValuesJson { get; set; } = "[]";
        public string GroupsJSON { get; set; } = "[]";
        public string SubgroupsJSON { get; set; } = "[]";
        public string LijnenJSON { get; set; } = "[]";
        public List<ParticipantEntity> Participants { get; set; } = new();

        internal MatchModel ToModel()
        {
            return new()
            {
                ArrowsPerEnd = ArrowsPerEnd,
                AutoProgressAfterEachArrow = AutoProgressAfterEachArrow,
                Groups = JsonConvert.DeserializeObject<List<GroupInfo>>(GroupsJSON) ?? new(),
                Subgroups = JsonConvert.DeserializeObject<List<GroupInfo>>(SubgroupsJSON) ?? new(),
                Lijnen = JsonConvert.DeserializeObject<List<string>>(LijnenJSON) ?? new(),
                Id = Id ?? -1,
                IsActiveMatch = false,
                MatchCode = MatchCode,
                MatchName = MatchName,
                NumberOfEnds = NumberOfEnds,
                ScoreValues = JsonConvert.DeserializeObject<Dictionary<string, List<ScoreButtonDefinition>>>(ScoreValuesJson) ?? new(),
            };
        }

        internal void UpdateFromModel(MatchModel match)
        {
            ArrowsPerEnd = match.ArrowsPerEnd;
            AutoProgressAfterEachArrow = match.AutoProgressAfterEachArrow;
            GroupsJSON = JsonConvert.SerializeObject(match.Groups);
            LijnenJSON = JsonConvert.SerializeObject(match.Lijnen);
            NumberOfEnds = match.NumberOfEnds;
            ScoreValuesJson = JsonConvert.SerializeObject(match.ScoreValues);
            SubgroupsJSON = JsonConvert.SerializeObject(match.Subgroups);
            MatchCode = match.MatchCode;
            MatchName = match.MatchName;
        }
    }
}
