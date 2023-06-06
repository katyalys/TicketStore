using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
		Task<IdentityUser> GetByIdAsync(string id);
		Task<Dictionary<IdentityUser, List<string>>> GetAllUsersWithRolesAsync();
		Task AddAsync(IdentityUser entity, string password);
		Task<(IdentityUser user, string newRole)> UpdateUserRoleAsync(IdentityUser user, string newRole);
		Task DeleteAsync(IdentityUser entity);
		Task<bool> CheckIfExists(IdentityUser entity);
		Task AddClaimssAsync(IdentityUser entity);
	}
}
