using Identity.Application.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Interfaces
{
    public interface IUserStorageProvider
    {
		Task<IdentityUser> GetByIdAsync(string id);
		Task<List<UserWithRoles>> GetAllUsersWithRolesAsync();
		Task<List<IdentityResult>> AddAsync(IdentityUser entity, string password);
		Task<(IdentityUser user, string newRole)> UpdateUserRoleAsync(IdentityUser user, string newRole);
		Task DeleteAsync(IdentityUser entity);
		Task<bool> CheckIfExists(IdentityUser entity);
		Task AddClaimssAsync(IdentityUser entity);
		Task<IList<string>> GetUserRole(string userId);
	}
}
