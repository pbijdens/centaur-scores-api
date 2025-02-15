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
        /// If set to true, this member is no longer active and should be considered deactivated.
        /// Their competition scores and other data will not be removed, but they will not be
        /// shown anymore in member lists and member selection panels, nor in personal best lists.
        /// </summary>
        public bool IsDeactivated { get; set; } = false;

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

        /// <summary>Personal best records for this member.</summary>
        public List<PersonalBestsListEntryEntity> PersonalBests { get; set; } = [];

        public ParticipantListMemberModel ToModel()
        {
            ParticipantListMemberModel result = new()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                Subgroup = Subgroup,
                IsDeactivated = IsDeactivated,
            };
            if (PersonalBests != null)
            {
                foreach (PersonalBestsListEntryEntity item in PersonalBests)
                {
                    if (item.Score > 0)
                    {
                        PersonalBestListEntryModel model = item.ToModel(false);
                        model.ListName = item.List?.Name;
                        result.PersonalBests.Add(model);
                    }
                }
            }
            return result;
        }

        public void UpdateFromModel(ParticipantListMemberModel metadata)
        {
            Name = metadata.Name;
            Group = metadata.Group;
            Subgroup = metadata.Subgroup;
            IsDeactivated = metadata.IsDeactivated;
        }
    }
}
