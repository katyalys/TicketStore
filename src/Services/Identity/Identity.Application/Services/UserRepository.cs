using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Application.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _identityUser;
        public UserRepository(UserManager<IdentityUser> identityUser)
        {
            _identityUser = identityUser;
        }

        public async Task<bool> CheckIfExists(IdentityUser entity)
        {
            var isExist = await _identityUser.Users.AnyAsync(x => x.Email == entity.Email || x.UserName == entity.UserName)
              .ConfigureAwait(false);

            return isExist;
        }

        public async Task AddClaimssAsync(IdentityUser entity)
        {
            string rolename = (await _identityUser.GetRolesAsync(entity)).FirstOrDefault();

            await _identityUser.AddClaimAsync(entity, new Claim("role", rolename));
            //await _identityUser.AddClaimAsync(entity, new Claim(ClaimTypes.Name, entity.UserName));
            //await _identityUser.AddClaimAsync(entity, new Claim(ClaimTypes.Email, entity.Email));
            //await _identityUser.AddClaimAsync(entity, new Claim(ClaimTypes.Role, rolename));
        }

        public async Task AddAsync(IdentityUser entity, string password)
        {
            await _identityUser.CreateAsync(entity, password);
            await _identityUser.AddToRoleAsync(entity, "customer");

        }

        public async Task DeleteAsync(IdentityUser entity)
        {
            await _identityUser.SetLockoutEnabledAsync(entity, true);
        }

        public async Task<IdentityUser> GetByIdAsync(string id)
        {
            return await _identityUser.FindByIdAsync(id);
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            var user = await GetByIdAsync(userId);
            return await _identityUser.GetRolesAsync(user);
        }

        public async Task<Dictionary<IdentityUser, List<string>>> GetAllUsersWithRolesAsync()
        {
            var users = await _identityUser.Users.Where(activeUsers => activeUsers.LockoutEnabled == false).ToListAsync();

            var usersWithRoles = new Dictionary<IdentityUser, List<string>>();

            foreach (var user in users)
            {
                var roles = await _identityUser.GetRolesAsync(user);
                usersWithRoles.Add(user, roles.ToList());
            }

            return usersWithRoles;
        }

        public async Task<(IdentityUser user, string newRole)> UpdateUserRoleAsync(IdentityUser user, string newRole)
        {
            var currentRoles = await _identityUser.GetRolesAsync(user);
            var result = await _identityUser.RemoveFromRolesAsync(user, currentRoles);

            if (result.Succeeded)
            {
                await _identityUser.AddToRoleAsync(user, newRole);
                return (user, newRole);
            }

            return (null, null);
        }
    }
}
