using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    public class CompetitionEntity
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string? RulesetGroupName { get; set; } = null;
        public DateOnly? StartDate { get; set; } = null;
        public DateOnly? EndDate { get; set; } = null;
        public ParticipantListEntity? ParticipantList { get; set; } = null;
        public List<MatchEntity> Matches { get; set; } = [];

        internal CompetitionModel ToMetadataModel()
        {
            CompetitionModel result = new()
            {
                Id = Id,
                Name = Name,
                RulesetGroupName = RulesetGroupName,
                StartDate = StartDate,
                EndDate = EndDate,
            };
            return result;
        }

        internal void UpdateMetadataFromModel(CompetitionModel metadata)
        {
            Name = metadata.Name;
            RulesetGroupName = RulesetGroupName;
            StartDate = StartDate;
            EndDate = EndDate;
        }
    }
}
