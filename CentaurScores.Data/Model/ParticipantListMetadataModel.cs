namespace CentaurScores.Model
{
    /// <summary>
    /// Metadata for a participant list.
    /// </summary>
    public class ParticipantListMetadataModel
    {
        /// <summary>
        /// The list ID.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
