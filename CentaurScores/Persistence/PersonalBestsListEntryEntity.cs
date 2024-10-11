using CentaurScores.Model;
using System.Xml.Linq;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// An entry in a personal best list.
    /// </summary>
    public class PersonalBestsListEntryEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>List this entry belongs to.</summary>
        public required PersonalBestsListEntity List { get; set; }

        /// <summary>Defines the archer to whocm this personal best belongs. 
        /// Nullable at model level required at database level.</summary>
        public required ParticipantListEntryEntity Participant { get; set; }

        /// <summary>
        /// Which discipline was this achieved in?
        /// </summary>
        public required string Discipline { get; set; }

        /// <summary>Date at which this score was achieved.</summary>
        public DateTimeOffset AchievedDate { get; set; }

        /// <summary>Score that was achieved.</summary>
        public int Score { get; set; }

        /// <summary>Additional notes, e.g. a description of the match at which this was achieved.</summary>
        public string Notes { get; set; } = string.Empty;

        internal PersonalBestListEntryModel ToModel()
        {
            PersonalBestListEntryModel model = new()
            {
                Id = Id,
                Participant = Participant?.ToModel() ?? new ParticipantListMemberModel { Id = -1 },
                Discipline = Discipline,
                Score = Score,
                Achieved = AchievedDate.ToString("yyyy-MM-dd"),
                Notes = Notes
            };
            return model;
        }

        internal void UpdateFromModel(CentaurScoresDbContext db, PersonalBestListEntryModel model)
        {
            Participant = db.ParticipantListEntries.Single(x => x.Id == model.Participant.Id);
            AchievedDate = DateTimeOffset.Parse(model.Achieved + "T00:00:00Z");
            Discipline = model.Discipline;
            Notes = model.Notes;
            Score = model.Score;
        }
    }
}
