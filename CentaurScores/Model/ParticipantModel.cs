namespace CentaurScores.Model
{
    public class ParticipantModel
    {
        public int Id { get; set; } = -1;
        public string Lijn { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Group{ get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int Score { get; set; } = -1;
        public List<EndModel> Ends { get; set; } = new();
        public string DeviceID { get; set; } = string.Empty;

    }
}
