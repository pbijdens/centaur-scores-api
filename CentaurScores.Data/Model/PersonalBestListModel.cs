using CentaurScores.Persistence;

namespace CentaurScores.Model
{
    /// <summary>
    /// Model representing a personal best list.
    /// </summary>
    public class PersonalBestListModel
    {
        /// <summary>
        /// Database ID or null.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Competition format string for the list.
        /// </summary>
        public string CompetitionFormat { get; set; } = string.Empty;

        /// <summary>
        /// Normally never set uinless the full list is fetched.
        /// </summary>
        public List<PersonalBestListEntryModel> Entries { get; set; } = [];
    }
}
