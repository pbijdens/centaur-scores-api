namespace CentaurScores.Persistence
{
    public class ParticipantListEntryEntity
    {
        public int? Id { get; set; } = null;
        public required ParticipantListEntity List { get; set; }
        /// <summary>This is the name of the participant.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>A default group for the participant.</summary>
        public string Group { get; set; } = string.Empty;
        /// <summary>A default sub-group for the participant.</summary>
        public string Subgroup { get; set; } = string.Empty;
    }
}
