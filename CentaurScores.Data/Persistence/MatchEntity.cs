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
        /// Code for the ruleset within the competition's ruleset group for the rules used in this game.
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
        /// The total number of rounds for this match; take into account rounds are only relevant for
        /// finals.
        /// </summary>
        public int NumberOfRounds {  get; set; } = 4;

        public MatchModel ToModel()
        {
            ListConfigurationModel? configuration = Competition?.ParticipantList?.GetConfiguration() ?? ListConfigurationModel.CentaurIndoorDefaults;
            List<GroupInfo> groups = configuration.Disciplines.OfType<GroupInfo>().ToList();
            List<GroupInfo> subgroups = configuration.Divisions.OfType<GroupInfo>().ToList();
            List<GroupInfo> targets = configuration.Targets.OfType<GroupInfo>().ToList();
            Dictionary<string, List<ScoreButtonDefinition>> scoreValues = configuration.Targets.Select(x => new KeyValuePair<string, List<ScoreButtonDefinition>>(
                x.Code,
                x.Keyboard.ToList()
                )).ToDictionary(x => x.Key, x => x.Value);

            List<GroupInfo> matchLevelGroups = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(GroupsJSON) ?? [];
            List<GroupInfo> matchLevelSubgroups = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(SubgroupsJSON) ?? [];
            List<GroupInfo> MatchLevelTargets = System.Text.Json.JsonSerializer.Deserialize<List<GroupInfo>>(TargetsJSON) ?? [];

            // We can now
            return new()
            {
                ArrowsPerEnd = ArrowsPerEnd,
                AutoProgressAfterEachArrow = AutoProgressAfterEachArrow,
                Groups = matchLevelGroups.Count == 0 ? groups : matchLevelGroups,
                Subgroups = matchLevelGroups.Count == 0 ? subgroups : matchLevelSubgroups,
                Targets = matchLevelGroups.Count == 0 ? targets : MatchLevelTargets,
                Lijnen = JsonConvert.DeserializeObject<List<string>>(LijnenJSON) ?? [],
                Id = Id ?? -1,
                IsActiveMatch = false,
                MatchCode = MatchCode,
                MatchName = MatchName,
                NumberOfEnds = NumberOfEnds,
                ScoreValues = matchLevelGroups.Count == 0 ? scoreValues : (JsonConvert.DeserializeObject<Dictionary<string, List<ScoreButtonDefinition>>>(ScoreValuesJson) ?? []),
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
