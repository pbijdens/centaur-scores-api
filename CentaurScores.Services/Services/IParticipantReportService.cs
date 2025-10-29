using CentaurScores.Model;

namespace CentaurScores.Services;

public interface IParticipantReportService
{
    /// <summary>
    /// Returns a full participant report for this list discipline, per (active) competition.
    /// </summary>
    /// <param name="listId"></param>
    /// <returns></returns>
    Task<List<ParticipantReport>> GetParticipantReportPerDiscipline(int listId, bool activeOnly = true);
}