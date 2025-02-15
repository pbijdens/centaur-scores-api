namespace CentaurScores.Model
{
    /// <summary>
    /// In case you forgot who you are.
    /// </summary>
    public class WhoAmIResponse
    {
        /// <summary>
        /// The numeric user ID for your user record.
        /// </summary>
        public int Id { get; set; } = -1;
        /// <summary>
        /// These key-value pairs should help resolve your identity crisis.
        /// </summary>
        public Dictionary<string, string> Claims { get; set; } = [];

    }
}
