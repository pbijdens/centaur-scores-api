using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB Entity for Accounts
    /// </summary>
    public class AccountEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Username, less than 32 characters long.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// A 4-character salt followed by a 64-character SHA256 hash of the concatenation of the salt 
        /// and the password for the user, so:
        /// To populate the record: 
        ///   <pre>SaltedPasswordHash := Salt + SHA256(CONCAT(Salt, Password));</pre>
        /// To verify a user password:
        ///   <pre>Salt := SUBSTRING(SaltedPasswordHash, 0, 4); 
        ///   IsSuccesfulLoginAttempt := SaltedPasswordHash == CONCAT(Salt, SHA256(CONCAT(Salt, TypedPassword)))</pre>
        /// </summary>
        public string SaltedPasswordHash { get; set; } = String.Empty;

        /// <summary>
        /// A list of ACL entities that this user is added to. In this system, ACLs explicitly specify all users.
        /// </summary>
        public List<AclEntity> ACLs { get; set; } = []; // List of ACLs that the user belongs to

        internal UserModel ToModel()
        {
            return new()
            {
                Id = Id,
                Username = Username,
                Acls = ACLs.Select(a => a.ToModel()).ToList(),
            };
        }
    }
}
