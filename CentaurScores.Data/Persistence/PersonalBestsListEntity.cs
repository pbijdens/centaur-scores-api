using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// Represents a list of personal best scores.
    /// </summary>
    public class PersonalBestsListEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// Descriptive name, such as "Centaur personal bests for 25m 1p indoor"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identifies the competition format (use tostring) for which entries may be 
        /// added to this list.
        /// </summary>
        public required string CompetitionFormat { get; set; } = string.Empty;

        /// <summary>
        /// List of entries.
        /// </summary>
        public List<PersonalBestsListEntryEntity> Entries { get; set; } = [];

        /// <summary>
        /// The only participant list that may contribute participants to this list.
        /// Required at database level, nullable at model level.
        /// </summary>
        public ParticipantListEntity? ParticipantList { get; set; }

        public PersonalBestListModel ToModel()
        {
            PersonalBestListModel model = new()
            {
                Id = Id,
                Name = Name,
                CompetitionFormat = CompetitionFormat,
            };
            return model;
        }

        public void UpdateFromModel(PersonalBestListModel model)
        {
            Name = model.Name;
            CompetitionFormat = model.CompetitionFormat;
        }
    }
}
