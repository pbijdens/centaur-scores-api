using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    public class ParticipantListEntity
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public List<ParticipantListEntryEntity> Entries { get; set; } = [];
        public List<CompetitionEntity> Competitions { get; set; } = [];

        internal ParticipantListModel ToModel()
        {
            ParticipantListModel result = new()
            {
                Id = Id,
                Name = Name,
                Entries = (Entries ?? []).Select(x => x.ToModel()).ToList(),
            };
            return result;
        }

        internal ParticipantListMetadataModel ToMetadataModel()
        {
            ParticipantListMetadataModel result = new()
            {
                Id = Id,
                Name = Name,
            };
            return result;
        }

        internal void UpdateFromModel(ParticipantListModel metadata)
        {
            Name = metadata.Name;
        }
    }
}
