
using Microsoft.Extensions.Configuration;

namespace CentaurScores.Services
{
    /// <summary>
    /// Database service realization for MYSQL
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class MySQLDatabaseService(IConfiguration configuration) : IDatabaseServices
    {

        /// <summary>
        /// Returns the database backup as a stream.
        /// </summary>
        public async Task<MemoryStream> GetDatabaseBackup()
        {
            string? connectionString = configuration.GetConnectionString("CentaurScoresDatabase");
            using MySql.Data.MySqlClient.MySqlConnection dbConnection = new(connectionString);
            using MySql.Data.MySqlClient.MySqlCommand sqlCommand = new();
            using MySql.Data.MySqlClient.MySqlBackup backupClient = new(sqlCommand);
            MemoryStream ms = new();

            sqlCommand.Connection = dbConnection;
            dbConnection.Open();
            backupClient.ExportToMemoryStream(ms);
            dbConnection.Close();

            ms.Position = 0;
            return await Task.FromResult(ms);
        }
    }
}
