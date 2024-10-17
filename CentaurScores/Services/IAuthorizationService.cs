using CentaurScores.Model;
using System.Security.Claims;

namespace CentaurScores.Services
{
    /// <summary>
    /// Authorization-related methods
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <returns></returns>
        Task<List<UserModel>> GetUsers();
        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserModel> CreateUser(ClaimsPrincipal loggedInUser, UserModel model);
        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteUser(ClaimsPrincipal loggedInUser, int userId);
        /// <summary>
        /// Update a user record.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserModel> UpdateUser(ClaimsPrincipal loggedInUser, UserModel model);

        /// <summary>
        /// Returns all ACLs.
        /// </summary>
        /// <returns></returns>
        Task<List<UserACLModel>> GetAcls();
        /// <summary>
        /// Create a new ACL.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserACLModel> CreateACL(ClaimsPrincipal loggedInUser, UserACLModel model);
        /// <summary>
        /// Delete an ACL.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteACL(ClaimsPrincipal loggedInUser, int userId);
        /// <summary>
        /// Update an ACL record.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserACLModel> UpdateACL(ClaimsPrincipal loggedInUser, UserACLModel model);
        /// <summary>
        /// Update the password for a user, requires knowledge of the current password.
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserModel> UpdatePassword(ClaimsPrincipal loggedInUser, UserModel model);
    }
}
