using CentaurScores.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB Match
    /// </summary>
    public class MatchEntity
    {
        /// <summary>Key</summary>
        public int? Id { get; set; }

        /// <summary>
        /// Code for the match, not unique, just used for sorting.
        /// </summary>
        public string MatchCode { get; set; } = string.Empty;
        /// <summary>
        /// Name of the match.
        /// </summary>
        public string MatchName { get; set; } = string.Empty;
        /// <summary>
        /// Number of ends.
        /// </summary>
        public int NumberOfEnds { get; set; } = -1;
        /// <summary>
        /// Arrows per end.
        /// </summary>
        public int ArrowsPerEnd { get; set; } = -1;
        /// <summary>
        /// Not used.
        /// </summary>
        public bool AutoProgressAfterEachArrow { get; set; } = false;
        /// <summary>
        /// JSON encoded dictionary of 'target code' x array[score button]
        /// </summary>
        public string ScoreValuesJson { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating allowed groups.
        /// </summary>
        public string GroupsJSON { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating allowed subgroups.
        /// </summary>
        public string SubgroupsJSON { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating targets.
        /// </summary>
        public string TargetsJSON { get; set; } = "[]";

        /// <summary>
        /// Lijnen as single-character array (json).
        /// </summary>
        public string LijnenJSON { get; set; } = "[]";

        /// <summary>
        /// All articipants for the match.
        /// </summary>
        public List<ParticipantEntity> Participants { get; set; } = [];

        /// <summary>
        /// Competition for the match.
        /// </summary>
        public CompetitionEntity? Competition { get; set; } = null;

        /// <summary>
        /// Code for the ruleset.
        /// </summary>
        public string? RulesetCode { get; set; } = null;

        /// <summary>
        /// Not used.
        /// </summary>
        public bool? ChangedRemotely {  get; set; } = false;

        internal MatchModel ToModel()
        {
            return new()
            {
                ArrowsPerEnd = ArrowsPerEnd,
                AutoProgressAfterEachArrow = AutoProgressAfterEachArrow,
                Groups = JsonConvert.DeserializeObject<List<GroupInfo>>(GroupsJSON) ?? [],
                Subgroups = JsonConvert.DeserializeObject<List<GroupInfo>>(SubgroupsJSON) ?? [],
                Targets = JsonConvert.DeserializeObject<List<GroupInfo>>(TargetsJSON) ?? [],
                Lijnen = JsonConvert.DeserializeObject<List<string>>(LijnenJSON) ?? [],
                Id = Id ?? -1,
                IsActiveMatch = false,
                MatchCode = MatchCode,
                MatchName = MatchName,
                NumberOfEnds = NumberOfEnds,
                ScoreValues = JsonConvert.DeserializeObject<Dictionary<string, List<ScoreButtonDefinition>>>(ScoreValuesJson) ?? [],
                RulesetCode = RulesetCode,
                Competition = Competition?.ToMetadataModel(),
                ChangedRemotely = ChangedRemotely ?? false,
            };
        }
    }
}
