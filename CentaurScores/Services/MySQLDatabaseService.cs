
namespace CentaurScores.Services
{
    public class MySQLDatabaseService : IDatabaseServices
    {
        private readonly IConfiguration configuration;

        public MySQLDatabaseService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<MemoryStream> GetDatabaseBackup()
        {
            string? connectionString = configuration.GetConnectionString("CentaurScoresDatabase");
            using (MySql.Data.MySqlClient.MySqlConnection dbConnection = new(connectionString))
            {
                using (MySql.Data.MySqlClient.MySqlCommand sqlCommand = new())
                {
                    using (MySql.Data.MySqlClient.MySqlBackup backupClient = new(sqlCommand))
                    {
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
        }
    }
}
