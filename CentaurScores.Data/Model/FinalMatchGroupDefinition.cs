namespace CentaurScores.Model
{
    /// <summary>
    /// Define a group of participants to the finals
    /// </summary>
    public class FinalMatchGroupDefinition
    {
        /// <summary>
        /// Name for this group
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// True if matches for thsi group are set-scored.
        /// </summary>
        public bool IsSetScored { get; set; } = true;

        /// <summary>
        /// List of zero or more participants in this group.
        /// </summary>
        public List<FinalMatchParticipantDefinition> Participants { get; set; } = [];
    }
}