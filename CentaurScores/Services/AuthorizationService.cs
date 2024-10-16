using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace CentaurScores.Services
{
    /// <summary>
    /// Authorization service
    /// </summary>
    public class AuthorizationService(IConfiguration configuration, IOptions<AppSettings> appSettings) : IAuthorizationService
    {
        /// <inheritdoc/>
        public async Task<UserACLModel> CreateACL(ClaimsPrincipal loggedInUser, UserACLModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            EnsureLoggedInUserIsAdmin(db, loggedInUser);
            AclEntity entity = new() { Name = model.Name };
            EntityEntry<AclEntity> createdEntityEntry = await db.ACLs.AddAsync(entity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return (await db.ACLs.SingleAsync(x => x.Id == createdObjectId)).ToModel();
        }

        /// <inheritdoc/>
        public async Task<UserModel> CreateUser(ClaimsPrincipal loggedInUser, UserModel model)
        {
            string salt = $"{Guid.NewGuid()}".Substring(0, 4);
            string newHashedPassword = salt + TokenService.CalculateSha256Hash(salt + model.Password);
            using var db = new CentaurScoresDbContext(configuration);
            EnsureLoggedInUserIsAdmin(db, loggedInUser);
            AccountEntity entity = new() { Username = model.Username, SaltedPasswordHash = newHashedPassword};
            if (model.Acls != null && entity.ACLs != null)
            {
                foreach (var acl in model.Acls ?? [])
                {
                    if (!entity.ACLs!.Any(a => a.Id == acl.Id))
                    {
                        entity.ACLs!.Add(db.ACLs.Single(dbACL => dbACL.Id == acl.Id));
                    }
                }
            }
            EntityEntry<AccountEntity> createdEntityEntry = await db.Accounts.AddAsync(entity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return (await db.Accounts.SingleAsync(x => x.Id == createdObjectId)).ToModel();
        }

        /// <inheritdoc/>
        public async Task<int> DeleteACL(ClaimsPrincipal loggedInUser, int aclId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            EnsureLoggedInUserIsAdmin(db, loggedInUser);
            AclEntity aclEntity = db.ACLs.SingleOrDefault(a => a.Id == aclId) ?? throw new ArgumentException("Bad ID", nameof(aclId));
            db.ACLs.Remove(aclEntity);

            AccountEntity accountEntity = db.Accounts.SingleOrDefault(a => a.Id == GetUserIDForLoggedInUser(loggedInUser)) ?? throw new ArgumentException("Bad ID", nameof(loggedInUser));
            EnsureUserIsStillAdministrator(loggedInUser, db, accountEntity); // throws on failure

            await db.SaveChangesAsync();
            return 1;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteUser(ClaimsPrincipal loggedInUser, int userId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            EnsureLoggedInUserIsAdmin(db, loggedInUser);

            if (GetUserIDForLoggedInUser(loggedInUser) == userId)
            {
                throw new ArgumentException("Can't delete your own account", nameof(userId));
            }

            AccountEntity accountEntity = db.Accounts.SingleOrDefault(a => a.Id == userId) ?? throw new ArgumentException("Bad ID", nameof(userId));
            db.Accounts.Remove(accountEntity);
            await db.SaveChangesAsync();
            return 1;
        }

        /// <inheritdoc/>
        public async Task<List<UserACLModel>> GetAcls()
        {
            using var db = new CentaurScoresDbContext(configuration);
            var result = (await db.ACLs.AsNoTracking().ToListAsync()).Select(m => m.ToModel()).ToList();
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<UserModel>> GetUsers()
        {
            using var db = new CentaurScoresDbContext(configuration);
            var result = (await db.Accounts.Include(a => a.ACLs).AsNoTracking().ToListAsync()).Select(m => m.ToModel()).ToList();
            return result;
        }

        /// <inheritdoc/>
        public async Task<UserACLModel> UpdateACL(ClaimsPrincipal loggedInUser, UserACLModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            EnsureLoggedInUserIsAdmin(db, loggedInUser);
            AclEntity aclEntity = db.ACLs.SingleOrDefault(a => a.Id == model.Id) ?? throw new ArgumentException("Bad ID", nameof(model));
            aclEntity.Name = model.Name;
            await db.SaveChangesAsync();
            return db.ACLs.Single(a => a.Id == model.Id).ToModel();
        }

        /// <inheritdoc/>
        public async Task<UserModel> UpdateUser(ClaimsPrincipal loggedInUser, UserModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            AccountEntity accountEntity = db.Accounts.Include(a => a.ACLs).SingleOrDefault(a => a.Id == model.Id) ?? throw new ArgumentException("Bad ID", nameof(model));
            if (!string.IsNullOrWhiteSpace(model.Username) && accountEntity.Username != model.Username)
            {
                EnsureLoggedInUserIsAdmin(db, loggedInUser);
                if (db.Accounts.Where(n => n.Username == model.Username).Count() != 0)
                {
                    throw new ArgumentException("Duplicate username", nameof(model));
                }
                accountEntity.Username = model.Username;
            }
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                EnsureLoggedInUserIsAdminOrSelf(db, loggedInUser, model.Id);
                if (IsTargetUSerSameAsLoggedInUser(db, loggedInUser, model.Id) && accountEntity.SaltedPasswordHash.Length > 4)
                {
                    // If the user wants to change their own password and they are not an admin
                    // they need to provide proof they know the current password
                    CheckCurrentPassword(model, accountEntity);
                }
                string salt = $"{Guid.NewGuid()}".Substring(0, 4);
                string newHashedPassword = salt + TokenService.CalculateSha256Hash(salt + model.Password);
                accountEntity.SaltedPasswordHash = newHashedPassword;
            }
            if (model.Acls != null && accountEntity.ACLs != null)
            {
                bool hasChange = false;
                foreach (var acl in accountEntity.ACLs?.ToList() ?? [])
                {
                    if (!model.Acls.Any(a => a.Id == acl.Id))
                    {
                        hasChange = true;
                        accountEntity.ACLs!.Remove(acl);
                    }
                }
                foreach (var acl in model.Acls ?? [])
                {
                    if (!accountEntity.ACLs!.Any(a => a.Id == acl.Id))
                    {
                        hasChange = true;
                        accountEntity.ACLs!.Add(db.ACLs.Single(dbACL => dbACL.Id == acl.Id));
                    }
                }
                if (hasChange)
                {
                    EnsureLoggedInUserIsAdmin(db, loggedInUser);
                    EnsureUserIsStillAdministrator(loggedInUser, db, accountEntity);
                }
            }
            await db.SaveChangesAsync();
            return db.Accounts.Include(a => a.ACLs).Single(a => a.Id == model.Id).ToModel();
        }

        /// <inheritdoc/>
        public async Task<UserModel> UpdatePassword(ClaimsPrincipal loggedInUser, UserModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            AccountEntity accountEntity = db.Accounts.Include(a => a.ACLs).SingleOrDefault(a => a.Id == model.Id) ?? throw new ArgumentException("Bad ID", nameof(model));
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                EnsureLoggedInUserIsAdminOrSelf(db, loggedInUser, model.Id);
                if (IsTargetUSerSameAsLoggedInUser(db, loggedInUser, model.Id) && accountEntity.SaltedPasswordHash.Length > 4)
                {
                    // If the user wants to change their own password and they are not an admin
                    // they need to provide proof they know the current password
                    CheckCurrentPassword(model, accountEntity);
                }
                string salt = $"{Guid.NewGuid()}".Substring(0, 4);
                string newHashedPassword = salt + TokenService.CalculateSha256Hash(salt + model.Password);
                accountEntity.SaltedPasswordHash = newHashedPassword;
            }
            await db.SaveChangesAsync();
            return db.Accounts.Include(a => a.ACLs).Single(a => a.Id == model.Id).ToModel();
        }

        private void EnsureUserIsStillAdministrator(ClaimsPrincipal loggedInUser, CentaurScoresDbContext db, AccountEntity accountEntity)
        {
            if (IsTargetUSerSameAsLoggedInUser(db, loggedInUser, accountEntity.Id) &&
                (!(accountEntity.ACLs ?? []).Any(acl => acl.Id == appSettings.Value.AdminACLId)))
            {
                throw new UnauthorizedAccessException("You can't remove your own admin rights");
            }
        }

        private void EnsureLoggedInUserIsAdminOrSelf(CentaurScoresDbContext db, ClaimsPrincipal loggedInUser, int? id)
        {
            if (GetUserIDForLoggedInUser(loggedInUser) == id)
            {
                return;
            }
            if (!loggedInUser.Claims.Any(c => c.Type == TokenService.IsAdministratorClaim))
            {
                throw new UnauthorizedAccessException($"Not allowed");
            }
        }

        private static int GetUserIDForLoggedInUser(ClaimsPrincipal loggedInUser)
        {
            return int.Parse(loggedInUser.Claims.FirstOrDefault(c => c.Type == TokenService.IdClaim)?.Value ?? "-1");
        }

        private bool IsTargetUSerSameAsLoggedInUser(CentaurScoresDbContext db, ClaimsPrincipal loggedInUser, int? id)
        {
            if (loggedInUser.Claims.FirstOrDefault(c => c.Type == TokenService.IdClaim)?.Value == $"{id}")
            {
                return true;
            }
            return false;
        }

        private void EnsureLoggedInUserIsAdmin(CentaurScoresDbContext db, ClaimsPrincipal loggedInUser)
        {
            if (!loggedInUser.Claims.Any(c => c.Type == TokenService.IsAdministratorClaim))
            {
                throw new UnauthorizedAccessException($"Not allowed");
            }
        }

        private static void CheckCurrentPassword(UserModel model, AccountEntity accountEntity)
        {
            string salt = accountEntity.SaltedPasswordHash[..4];
            string expectedPassword = salt + TokenService.CalculateSha256Hash(salt + model.CurrentPassword);
            if (expectedPassword != accountEntity.SaltedPasswordHash)
            {
                throw new ArgumentException("Bad password", nameof(model));
            }
        }
    }
}
