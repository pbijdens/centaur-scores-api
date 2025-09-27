using CentaurScores.Persistence;

namespace CentaurScores.Model
{
    /// <summary>
    /// Represnts a full participant list with members.
    /// </summary>
    public class ParticipantListModel
    {
        /// <summary>
        /// ID of the list.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the list is inactive or not.
        /// </summary>
        public bool IsInactive { get; set; } = false;

        /// <summary>
        /// All entries in the list.
        /// </summary>
        public List<ParticipantListMemberModel> Entries { get; set; } = [];
    }
}
