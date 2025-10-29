using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.Model;

public class ParticipantReport
{
    public string Discipline { get; set; } = string.Empty;

    public List<ParticipantReportEntry> Participants { get; set; } = new();

    public List<string> ActiveCompetitions { get; set; } = new();

    public class ParticipantReportEntry
    {
        public int MemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ParticipantCompetitionReportEntry?> Competitions { get; set; } = new();
    }

    public class ParticipantCompetitionReportEntry
    {
        public string Division { get; set; } = string.Empty;
        public int MatchesPlayed { get; set; }
        public int ArrowsShot { get; set; }
        public int ArrowTotal { get; set; }
        public double PerArrowAverage { get; set; }
    }
}
