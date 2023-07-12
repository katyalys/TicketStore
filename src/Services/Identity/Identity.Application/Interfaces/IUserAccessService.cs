using Identity.Application.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Interfaces
{
    public interface IUserAccessService
    {
		Task<IdentityUser> GetByIdAsync(string id);
		Task<List<UserWithRoles>> GetAllUsersWithRolesAsync();
		Task<List<IdentityResult>> AddAsync(IdentityUser entity, string password);
		Task<(IdentityUser user, string newRole)> UpdateUserRoleAsync(IdentityUser user, string newRole);
		Task DeleteAsync(IdentityUser entity);
		Task<bool> CheckIfExistsAsync(IdentityUser entity);
		Task AddClaimssAsync(IdentityUser entity);
		Task<IList<string>> GetUserRoleAsync(string userId);
	}
}
