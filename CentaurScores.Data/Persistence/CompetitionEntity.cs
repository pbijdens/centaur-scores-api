using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB Competition
    /// </summary>
    public class CompetitionEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// Name of the record
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Name of the ruleset group that's used to decide how to score this competition.
        /// </summary>
        public string? RulesetGroupName { get; set; } = null;

        /// <summary>
        /// Optional parameters that may be used during ruleset processing.
        /// </summary>
        public string? RulesetParametersJSON { get; set; } = null;

        /// <summary>
        /// Start date, optional.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; } = null;

        /// <summary>
        /// End date, optional.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; } = null;

        /// <summary>
        /// Participant list that can be used to link participants of the competition together.
        /// </summary>
        public ParticipantListEntity? ParticipantList { get; set; } = null;

        /// <summary>
        /// All matches of the competition.
        /// </summary>
        public List<MatchEntity> Matches { get; set; } = [];
        /// <summary>
        /// If unset, assume false. If set to true, the list is inactive and typically not shown.
        /// </summary>
        public bool? IsInactive { get; set; } = null;

        public CompetitionModel ToMetadataModel()
        {
            CompetitionModel result = new()
            {
                Id = Id,
                Name = Name,
                RulesetGroupName = RulesetGroupName,
                RulesetParametersJSON = RulesetParametersJSON,
                StartDate = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : null,
                EndDate = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null,
                ParticipantsList = ParticipantList?.ToMetadataModel(),
                IsInactive = IsInactive ?? false,
            };
            return result;
        }

        public void UpdateMetadataFromModel(CompetitionModel metadata)
        {
            Name = metadata.Name;
            RulesetGroupName = metadata.RulesetGroupName;
            RulesetParametersJSON = metadata.RulesetParametersJSON;
            StartDate = metadata.StartDate == null ? null : DateTimeOffset.Parse(metadata.StartDate + "T00:00:00Z");
            EndDate = metadata.EndDate == null ? null : DateTimeOffset.Parse(metadata.EndDate + "T00:00:00Z");
            IsInactive = metadata.IsInactive;
        }
    }
}
