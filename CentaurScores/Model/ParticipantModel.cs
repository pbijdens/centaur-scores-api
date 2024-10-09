
namespace CentaurScores.Model
{
    /// <summary>
    /// Model representing a single participant of a single match.
    /// </summary>
    public class ParticipantModel
    {
        /// <summary>
        /// Record ID.
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// Lijn that the participant occupies.
        /// </summary>
        public string Lijn { get; set; } = string.Empty;

        /// <summary>
        /// Name of the participant.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Group code for this participant.
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Subgroup code for this participant.
        /// </summary>
        public string Subgroup { get; set; } = string.Empty;

        /// <summary>
        /// Target used for the participant.
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// Total score, sum of all arrows that hit.
        /// </summary>
        public int Score { get; set; } = -1;

        /// <summary>
        /// List of ends for this match for this participant.
        /// </summary>
        public List<EndModel> Ends { get; set; } = [];

        /// <summary>
        /// Device ID for the device that manages this participant.
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// If not null, this is the ID of the participant list record for the participant list
        /// linked to the competition that this match is part of.
        /// </summary>
        public int? ParticipantListEntryId { get; set; } = null;

        /// <summary>
        /// Converts the record into a ParticipantData record for result-calculation.
        /// </summary>
        /// <returns></returns>
        internal ParticipantData ToData()
        {
            return new ParticipantData
            {
                Id = (!ParticipantListEntryId.HasValue || ParticipantListEntryId.Value < 0) ? -1 : ParticipantListEntryId.Value,
                Name = Name,
                Group = Group,
                Subgroup = Subgroup
            };
        }
    }
}
