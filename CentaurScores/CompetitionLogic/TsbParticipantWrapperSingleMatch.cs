using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    public class TsbParticipantWrapperSingleMatch
    {
        public required ParticipantModel Participant;
        public required IEnumerable<EndModel> Ends;
        public required string ClassCode;
        public required string SubclassCode;
        public int Score = 0;
        public Dictionary<int, int> Tiebreakers = [];
        public int TiebreakerArrow = int.MaxValue;
    }
}