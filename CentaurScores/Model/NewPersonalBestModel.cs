namespace CentaurScores.Model
{

    /// <summary>
    /// Model representing a single archer in a personal best list.
    /// </summary>
    public class NewPersonalBestModel
    {
        /// <summary>
        /// Database ID or null if new.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public int ListId { get; set; } = 0;

        /// <summary>
        /// Competition in which the personal best was scored.
        /// </summary>
        public CompetitionModel Competition { get; set; } = new();

        /// <summary>
        /// Match for which the personal best is scored.
        /// </summary>
        public MatchModel Match { get; set; } = new();

        /// <summary>
        /// Reference to a participant.
        /// </summary>
        public ParticipantListMemberModel Participant { get; set; } = new();

        /// <summary>
        /// The discipline in which the record was achieved.
        /// </summary>
        public string Discipline { get; set; } = string.Empty;

        /// <summary>
        /// Current personal best score for the participant (within the list's scope).
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Previous personal best score for the participant (within the list's scope).
        /// </summary>
        public int PreviousScore { get; set; } = 0;

        /// <summary>
        /// Notes.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// When was this achieved?
        /// </summary>
        public string Achieved { get; set; } = string.Empty;
    }
}
