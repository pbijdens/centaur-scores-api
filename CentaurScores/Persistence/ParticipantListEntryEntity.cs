using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB entry in participant list
    /// </summary>
    public class ParticipantListEntryEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// List thsi entry belongs to
        /// </summary>
        public required ParticipantListEntity List { get; set; }
        
        /// <summary>This is the name of the participant.</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>A default group for the participant. Not a code but a label.</summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>A default sub-group for the participant. Not a code but a label.</summary>
        public string Subgroup { get; set; } = string.Empty;

        internal ParticipantListMemberModel ToModel()
        {
            ParticipantListMemberModel result = new()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                Subgroup = Subgroup
            };
            return result;
        }

        internal void UpdateFromModel(ParticipantListMemberModel metadata)
        {
            Name = metadata.Name;
            Group = metadata.Group;
            Subgroup = metadata.Subgroup;
        }
    }
}
