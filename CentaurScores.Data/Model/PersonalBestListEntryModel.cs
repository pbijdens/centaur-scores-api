namespace CentaurScores.Model
{
    /// <summary>
    /// Model representing a single archer in a personal best list.
    /// </summary>
    public class PersonalBestListEntryModel
    {
        /// <summary>
        /// Database ID or null.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// Reference to a participant.
        /// </summary>
        public ParticipantListMemberModel Participant { get; set; } = new();

        /// <summary>
        /// Discipline in which the personal best is achieved.
        /// </summary>
        public string Discipline { get; set; } = string.Empty;

        /// <summary>
        /// Current personal best score for the participant (within the list's scope).
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Notes.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// When was this achieved?
        /// </summary>
        public string Achieved { get; set; } = string.Empty;

        /// <summary>
        /// The name of the list. Optional.
        /// </summary>
        public string? ListName {  get; set; } = string.Empty;
    }
}
