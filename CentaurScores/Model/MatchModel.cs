using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace CentaurScores.Model
{
    public class MatchModel
    {
        public int Id { get; set; } = -1;
        public string DeviceID { get; set; } = string.Empty;
        public string WedstrijdCode { get; set; } = string.Empty;
        public string WedstrijdNaam { get; set; } = string.Empty;
        public int Ends { get; set; } = -1;
        public int ArrowsPerEnd { get; set; } = -1;
        public bool AutoProgressAfterEachArrow { get; set; } = false;
        public Dictionary<string, List<ScoreButtonDefinition>> ScoreValues { get; set; } = new();
        public List<GroupInfo> Groups { get; set; } = new();
        public List<GroupInfo> Subgroups { get; set; } = new();
        public List<ParticipantModel> Participants { get; set; } = new();

    }
}
