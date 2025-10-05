using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// Database participant list entity. This is actually more a 'tenant' or 'organization' concept, as it
    /// contains all participants that can be referenced from matches and personal best lists, as well as 
    /// generic configuration for all competitions within the list.
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

        /// <summary>
        /// If unset, assume false. If set to true, the list is inactive and typically not shown.
        /// </summary>
        public bool? IsInactive { get; set; } = null;

        /// <summary>
        /// This contains the configuration for this list. A parti
        /// </summary>
        public string? ConfigurationJSON { get; set; }

        public ParticipantListModel ToModel()
        {
            ParticipantListModel result = new()
            {
                Id = Id,
                Name = Name,
                Entries = (Entries ?? []).Select(x => x.ToModel()).ToList(),
                IsInactive = IsInactive ?? false,
                Configuration = this.GetConfiguration()
            };
            return result;
        }

        public ParticipantListMetadataModel ToMetadataModel()
        {
            ParticipantListMetadataModel result = new()
            {
                Id = Id,
                Name = Name,
                Configuration = this.GetConfiguration(),
            };
            return result;
        }

        public void UpdateFromModel(ParticipantListModel metadata)
        {
            Name = metadata.Name;
            IsInactive = metadata.IsInactive;
            ConfigurationJSON = System.Text.Json.JsonSerializer.Serialize(metadata.Configuration ?? ListConfigurationModel.Default);
        }
    }
}
