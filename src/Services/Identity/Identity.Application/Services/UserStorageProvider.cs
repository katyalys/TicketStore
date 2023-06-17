using AutoMapper;
using Identity.Application.Dtos;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Application.Services
{
    public class UserStorageProvider : IUserStorageProvider
    {

        private readonly UserManager<IdentityUser> _identityUser;
        private readonly IMapper _mapper;

        public UserStorageProvider(UserManager<IdentityUser> identityUser, IMapper mapper)
        {
            _identityUser = identityUser;
            _mapper = mapper;
        }

        public async Task<bool> CheckIfExists(IdentityUser entity)
        {
            var isExist = await _identityUser.Users.AnyAsync(x => x.Email == entity.Email || x.UserName == entity.UserName);
           
            return isExist;
        }

        public async Task AddClaimssAsync(IdentityUser entity)
        {
            string rolename = (await _identityUser.GetRolesAsync(entity)).FirstOrDefault();
            await _identityUser.AddClaimAsync(entity, new Claim("role", rolename));
        }

        public async Task<List<IdentityResult>> AddAsync(IdentityUser entity, string password)
        {
            var createResult = await _identityUser.CreateAsync(entity, password);
            var deleteResult = await _identityUser.SetLockoutEnabledAsync(entity, false);
            var roleResult = await _identityUser.AddToRoleAsync(entity, "Customer");
            var listResult = new List<IdentityResult>() { createResult, deleteResult, roleResult}; 

            return listResult;
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

        public async Task<List<UserWithRoles>> GetAllUsersWithRolesAsync()
        {
            var users = await _identityUser.Users.Where(activeUsers => activeUsers.LockoutEnabled == false).ToListAsync();

            var usersWithRoles = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _identityUser.GetRolesAsync(user);

                UserViewModel? tmpUser = _mapper.Map<UserViewModel>(user);
                var userWithRoles = new UserWithRoles
                {
                    User = _mapper.Map<UserViewModel>(user),
                    Roles = roles.ToList()
                };
                usersWithRoles.Add(userWithRoles);
            }

            return usersWithRoles;
        }

        public async Task<(IdentityUser user, string newRole)> UpdateUserRoleAsync(IdentityUser user, string newRole)
        {
            var currentRoles = await _identityUser.GetRolesAsync(user);
            var result = await _identityUser.RemoveFromRolesAsync(user, currentRoles);
            await _identityUser.AddToRoleAsync(user, newRole);

            return (user, newRole);
        }
    }
}
