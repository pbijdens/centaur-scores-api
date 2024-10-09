using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB participant list.
    /// </summary>
    public class ParticipantListEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;
        /// <summary>
        /// Name of the list
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// List of entries.
        /// </summary>
        public List<ParticipantListEntryEntity> Entries { get; set; } = [];
        /// <summary>
        /// List of competitions using the list.
        /// </summary>
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
