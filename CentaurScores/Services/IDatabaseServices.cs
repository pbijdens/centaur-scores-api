namespace CentaurScores.Services
{
    public interface IDatabaseServices
    {
        Task<MemoryStream> GetDatabaseBackup();
    }
}
