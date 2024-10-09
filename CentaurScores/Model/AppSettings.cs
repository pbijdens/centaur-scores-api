namespace CentaurScores.Model
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The secret used for signing JWT tokens.
        /// </summary>
        public string Secret { get; set; } = string.Empty;
        /// <summary>
        /// Needs to be added to the /admin/backup endpoint to 'authenticate' the request.
        /// </summary>
        public string BackupSecret { get; set; } = string.Empty;
        /// <summary>
        /// An ACL with tis ID must be present and is used to identify the admins of the system.
        /// </summary>
        public int AdminACLId { get; set; } = -1;
    }
}
