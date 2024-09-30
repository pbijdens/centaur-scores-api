using CentaurScores.Persistence;

namespace CentaurScores.Model
{
    public class ParticipantListModel
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public List<ParticipantListMemberModel> Entries { get; set; } = [];
    }
}
