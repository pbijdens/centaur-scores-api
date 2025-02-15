namespace CentaurScores.Model
{
    /// <summary>
    /// Extended participant model. See also <see cref="ParticipantModel"/>. Extended to add extra data for rendering.
    /// </summary>
    public class ParticipantModelV2 : ParticipantModel
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
    }
}
