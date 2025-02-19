namespace CentaurScores.Model
{
    public class ParticipantModelV3 : ParticipantModelV2
    {
        /// <summary>
        /// Name of the target for this participant.
        /// </summary>
        public List<HeadToHeadInfoEntry> H2HInfo { get; set; } = [];
    }
}
