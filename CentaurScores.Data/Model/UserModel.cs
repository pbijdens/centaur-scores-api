namespace CentaurScores.Model
{
    /// <summary>
    /// User
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Persistence ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Password, only set when changing the password
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Required when changing the password.
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;
        /// <summary>
        /// List of ACLs for the user
        /// </summary>
        public List<UserACLModel> Acls { get; set; } = [];
    }
}
