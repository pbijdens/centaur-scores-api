namespace CentaurScores.Model
{
    /// <summary>
    /// Login request data.
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// USername, case sensitive.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Password, case sensitive.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
