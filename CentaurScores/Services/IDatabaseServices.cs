namespace CentaurScores.Services
{
    /// <summary>
    /// Service for performing DB operations.
    /// </summary>
    public interface IDatabaseServices
    {
        /// <summary>
        /// Request a backup in a stream.
        /// </summary>
        /// <returns></returns>
        Task<MemoryStream> GetDatabaseBackup();
    }
}
