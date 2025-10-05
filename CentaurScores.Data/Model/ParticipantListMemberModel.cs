using CentaurScores.Persistence;

namespace CentaurScores.Model
{
    /// <summary>
    /// Record in a participant list.
    /// </summary>
    public class ParticipantListMemberModel
    {
        /// <summary>
        /// Record's ID.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>This is the name of the participant.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>A default group for the participant.</summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>A default sub-group for the participant.</summary>
        public string Subgroup { get; set; } = string.Empty;

        /// <summary>True indicates the member is deactivated..</summary>
        public bool IsDeactivated { get; set; } = false;

        /// <summary>If applicable, all personal best entries for this user. Only populated
        /// when a single participant list entry is retrieved.</summary>
        public List<PersonalBestListEntryModel> PersonalBests { get; set; } = [];

        /// <summary>
        /// Mapping from competition format + discipline to division.
        /// </summary>
        public List<CompetitionFormatDisciplineDivisionMapModel> CompetitionFormatDisciplineDivisionMap { get; set; } = [];
    }

    public class CompetitionFormatDisciplineDivisionMapModel
    {
        public string CompetitionFormat { get; set; } = string.Empty;
        public string DisciplineCode { get; set; } = string.Empty;
        public string? DivisionCode { get; set; } = string.Empty;
    }
}
