using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    public class CompetitionEntity
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        /// <summary>Name of the ruleset group that's used to decide how to score this competition.</summary>
        public string? RulesetGroupName { get; set; } = null;
        /// <summary>Optional parameters that may be used during ruleset processing.</summary>
        public string? RulesetParametersJSON { get; set; } = null;
        public DateTimeOffset? StartDate { get; set; } = null;
        public DateTimeOffset? EndDate { get; set; } = null;
        public ParticipantListEntity? ParticipantList { get; set; } = null;
        public List<MatchEntity> Matches { get; set; } = [];

        internal CompetitionModel ToMetadataModel()
        {
            CompetitionModel result = new()
            {
                Id = Id,
                Name = Name,
                RulesetGroupName = RulesetGroupName,
                RulesetParametersJSON = RulesetParametersJSON,
                StartDate = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : null,
                EndDate = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null,
                ParticipantsList = ParticipantList == null ? null : ParticipantList.ToMetadataModel()
            };
            return result;
        }

        internal void UpdateMetadataFromModel(CompetitionModel metadata)
        {
            Name = metadata.Name;
            RulesetGroupName = metadata.RulesetGroupName;
            RulesetParametersJSON = metadata.RulesetParametersJSON;
            StartDate = metadata.StartDate == null ? null : DateTimeOffset.Parse(metadata.StartDate + "T00:00:00Z");
            EndDate = metadata.EndDate == null ? null : DateTimeOffset.Parse(metadata.EndDate + "T00:00:00Z");
        }
    }
}
