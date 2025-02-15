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
        /// The personal best lists that are associated with this participant list.
        /// If a match 
        /// </summary>
        public List<PersonalBestsListEntity> PersonalBestLists { get; set; } = [];

        /// <summary>
        /// List of competitions using the list.
        /// </summary>
        public List<CompetitionEntity> Competitions { get; set; } = [];

        public ParticipantListModel ToModel()
        {
            ParticipantListModel result = new()
            {
                Id = Id,
                Name = Name,
                Entries = (Entries ?? []).Select(x => x.ToModel()).ToList(),
            };
            return result;
        }

        public ParticipantListMetadataModel ToMetadataModel()
        {
            ParticipantListMetadataModel result = new()
            {
                Id = Id,
                Name = Name,
            };
            return result;
        }

        public void UpdateFromModel(ParticipantListModel metadata)
        {
            Name = metadata.Name;
        }
    }
}
