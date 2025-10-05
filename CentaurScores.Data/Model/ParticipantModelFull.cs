namespace CentaurScores.Model
{
    public class ParticipantModelFull : ParticipantModelSimple
    {
        /// <summary>
        /// Label of the group info structure for the participants group.
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// Label of the subgroups'group info structure for the participant.
        /// </summary>
        public string? SubgroupName { get; set; }

        /// <summary>
        /// Name of the target for this participant.
        /// </summary>
        public string? TargetName { get; set; }
        /// <summary>
        /// Name of the target for this participant.
        /// </summary>
        public List<HeadToHeadInfoEntry> H2HInfo { get; set; } = [];
    }
}
