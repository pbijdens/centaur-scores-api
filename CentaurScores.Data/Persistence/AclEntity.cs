
using CentaurScores.Model;

namespace CentaurScores.Persistence
{
    /// <summary>
    /// Simplest possible ACL implementation. No inheritance, no rules, just included accounts.
    /// </summary>
    public class AclEntity
    {
        /// <summary>
        /// DB ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name of the ACL, unique.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of accounts belonging to this ACL.
        /// </summary>
        public List<AccountEntity> Accounts { get; set; } = [];

        public UserACLModel ToModel()
        {
            return new() {
                Id = Id,
                Name = Name,
            };
        }
    }
}
