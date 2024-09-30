namespace CentaurScores.Persistence
{
    public class ParticipantListEntity
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public List<ParticipantListEntryEntity> Entries { get; set; } = [];

    }
}
