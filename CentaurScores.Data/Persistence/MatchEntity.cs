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
        public const uint MatchFlagsNone = 0x0000;
        public const uint MatchFlagsHeadToHead = 0x0001;

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
        [Obsolete("Using the keyboards defined in LIST context")]
        public string ScoreValuesJson { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating allowed groups.
        /// </summary>
        [Obsolete("Using the groups defined in LIST context")]
        public string GroupsJSON { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating allowed subgroups.
        /// </summary>
        [Obsolete("Using the sub-groups defined in LIST context")]
        public string SubgroupsJSON { get; set; } = "[]";

        /// <summary>
        /// List of group info structures indicating targets.
        /// </summary>
        [Obsolete("Using the targets defined in LIST context")]
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

        /// <summary>
        /// Can be used to tweak properties of this match, for example to make this a final. The interpretation
        /// of the flags is partly up to the competition score calculation module.
        /// </summary>
        public uint MatchFlags { get; set; } = MatchFlagsNone;

        /// <summary>
        /// Currently active round for this match, only relevant for matches where multiple rounds are
        /// available. Can be used to split a single match into multiple rounds, provided the score
        /// calculator can merge these rounds again. Initially only used for finals.
        /// </summary>
        public int ActiveRound { get; set; } = 0;

        /// <summary>
        /// Currently active round for this match, only relevant for matches where multiple rounds are
        /// available. Can be used to split a single match into multiple rounds, provided the score
        /// calculator can merge these rounds again. Initially only used for finals.
        /// </summary>
        public int NumberOfRounds {  get; set; } = 4;

        public MatchModel ToModel()
        {
            ListConfigurationModel? configuration = Competition?.ParticipantList?.GetConfiguration() ?? ListConfigurationModel.Default;

            // todo: consolidate the match specific info with the configuration, in due time eliminate the match specific info entirely
#warning "HERE"
#warning "HERE"
#warning "HERE"
#warning "HERE - todo: consolidate the match specific info with the configuration, in due time eliminate the match specific info entirely"
#warning "HERE - make sure that all codes used currently are valid in the returned lists and try to map them to competition lists"
#warning "HERE - if the list is empty only use competition lists"

            List<GroupInfo> groups = configuration.Disciplines.OfType<GroupInfo>().ToList();
            List<GroupInfo> subgroups = configuration.Divisions.OfType<GroupInfo>().ToList();
            List<GroupInfo> targets = configuration.Targets.OfType<GroupInfo>().ToList();
            Dictionary<string, List<ScoreButtonDefinition>> scoreValues = configuration.Targets.Select(x => new KeyValuePair<string, List<ScoreButtonDefinition>>(
                x.Code,
                x.Keyboard.ToList()
                )).ToDictionary(x => x.Key, x => x.Value);

            List<GroupInfo> oldGroups = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(GroupsJSON) ?? [];
            List<GroupInfo> oldSubgroups = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(SubgroupsJSON) ?? [];
            List<GroupInfo> oldTargets = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(TargetsJSON) ?? [];
            Dictionary<string, List<ScoreButtonDefinition>> oldKeyboards = JsonConvert.DeserializeObject<Dictionary<string, List<ScoreButtonDefinition>>>(ScoreValuesJson) ?? [];

            // We can now
            return new()
            {
                ArrowsPerEnd = ArrowsPerEnd,
                AutoProgressAfterEachArrow = AutoProgressAfterEachArrow,
                Groups = oldGroups.Count == 0 ? groups : oldGroups,
                Subgroups = oldGroups.Count == 0 ? subgroups : oldSubgroups,
                Targets = oldGroups.Count == 0 ? targets : oldTargets,
                Lijnen = JsonConvert.DeserializeObject<List<string>>(LijnenJSON) ?? [],
                Id = Id ?? -1,
                IsActiveMatch = false,
                MatchCode = MatchCode,
                MatchName = MatchName,
                NumberOfEnds = NumberOfEnds,
                ScoreValues = oldGroups.Count == 0 ? scoreValues : (JsonConvert.DeserializeObject<Dictionary<string, List<ScoreButtonDefinition>>>(ScoreValuesJson) ?? []),
                RulesetCode = RulesetCode,
                Competition = Competition?.ToMetadataModel(),
                ChangedRemotely = ChangedRemotely ?? false,
                MatchFlags = MatchFlags,
                ActiveRound = ActiveRound,
                NumberOfRounds = NumberOfRounds,
            };
        }
    }
}
