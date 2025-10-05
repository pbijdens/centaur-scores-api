using CentaurScores.Model;
using MySqlX.XDevAPI.Common;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// A single participant that's on a list of potential participants for a competition. This is used for linking participants in 
    /// matches and personal best lists. A match can have participants that ar enot on the list, but only when the participant is
    /// mapped to this list they will show up in the competition results as the same person always.
    /// </summary>
    /// <remarks>
    /// Per participant per discilpline division mapping is stored here as JSON.
    /// </remarks>
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

        /// <summary>A default discipline for the participant. Should be a code.</summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>A default division for the participant. Should not be used anymore.</summary>
        public string Subgroup { get; set; } = string.Empty;

        /// <summary>Mapping for this user from (CompetitionFormat,Discipline) to a Division</summary>
        public string? CompetitionFormatDisciplineDivisionMapJSON { get; set; }

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
                        model.CompetitionFormat = item.List?.CompetitionFormat ?? string.Empty;
                        result.PersonalBests.Add(model);
                    }
                }
            }
            if (CompetitionFormatDisciplineDivisionMapJSON == null)
            {
                result.CompetitionFormatDisciplineDivisionMap = [];
            }
            else
            {
                result.CompetitionFormatDisciplineDivisionMap = System.Text.Json.JsonSerializer.Deserialize<List<CompetitionFormatDisciplineDivisionMapModel>>(CompetitionFormatDisciplineDivisionMapJSON) ?? [];
            }
            return result;
        }

        public void UpdateFromModel(ParticipantListMemberModel metadata)
        {
            Name = metadata.Name;
            Group = metadata.Group;
            Subgroup = metadata.Subgroup;
            IsDeactivated = metadata.IsDeactivated;
            CompetitionFormatDisciplineDivisionMapJSON = System.Text.Json.JsonSerializer.Serialize(metadata.CompetitionFormatDisciplineDivisionMap);
        }
    }
}
