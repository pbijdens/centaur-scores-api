using CentaurScores.Attributes;
using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Endpoints for user management.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("auth")]
    public class UserController(IAuthorizationService authorizationService) : ControllerBase
    {
        /// <summary>
        /// Returns the list of all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<List<UserModel>>> GetUsers()
        {
            return await authorizationService.GetUsers();
        }

        /// <summary>
        /// Returns the list of all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet("acl")]
        [Authorize]
        public async Task<ActionResult<List<UserACLModel>>> GetACLs()
        {
            return await authorizationService.GetAcls();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <returns></returns>
        [HttpPost("user")]
        [Authorize]
        public async Task<ActionResult<UserModel>> CreateUser(UserModel model)
        {
            return await authorizationService.CreateUser(HttpContext.User, model);
        }

        /// <summary>
        /// Creates a new ACL.
        /// </summary>
        /// <returns></returns>
        [HttpPost("acl")]
        [Authorize]
        public async Task<ActionResult<UserACLModel>> CreateACL(UserACLModel model)
        {
            return await authorizationService.CreateACL(HttpContext.User, model);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <returns></returns>
        [HttpPut("user")]
        [Authorize]
        public async Task<ActionResult<UserModel>> UpdateUser(UserModel model)
        {
            return await authorizationService.UpdateUser(HttpContext.User, model);
        }

        /// <summary>
        /// Updates an ACL.
        /// </summary>
        /// <returns></returns>
        [HttpPut("acl")]
        [Authorize]
        public async Task<ActionResult<UserACLModel>> UpdateACL(UserACLModel model)
        {
            return await authorizationService.UpdateACL(HttpContext.User, model);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteUser([FromRoute] int userId)
        {
            return await authorizationService.DeleteUser(HttpContext.User, userId);
        }

        /// <summary>
        /// Updates an ACL.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("acl/{aclId}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteACL([FromRoute] int aclId)
        {
            return await authorizationService.DeleteACL(HttpContext.User, aclId);
        }

    }
}
