using AutoMapper;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _identityUser;
        public UserService(IUserRepository identityUser, IMapper mapper)
        {
            _identityUser = identityUser;
            _mapper = mapper;
        }

        public async Task<int> RegisterCustomer(RegisterUser registerUser)
        {
            if (registerUser.Password == registerUser.ConfirmPassword)
            {
                var user = _mapper.Map<IdentityUser>(registerUser);
                var isExist = await _identityUser.CheckIfExists(user);

                if (isExist)
                    return 0;

                await _identityUser.AddAsync(user, registerUser.Password);
                await _identityUser.AddClaimssAsync(user);
            }
            return 0;
        }

        public async Task<bool> DeleteUser(string id) {
            var user = await _identityUser.GetByIdAsync(id);
            if (user != null) {
                await _identityUser.DeleteAsync(user);
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<UserWithRoles> ChangeRole(string id, string role)
        {
            var user = await _identityUser.GetByIdAsync(id);
            if (user != null)
            {
                var updatedUser = await _identityUser.UpdateUserRoleAsync(user, role);
                return new UserWithRoles
                {
                    User = _mapper.Map<UserViewModel>(updatedUser.user),
                    Roles = new List<string>() { updatedUser.newRole }
                };
            }
            else
            {
                throw new Exception("No such id");
            }
        }

        public async Task<List<UserWithRoles>> GetAllUsers()
        {
            var usersWithRoles = await _identityUser.GetAllUsersWithRolesAsync();

            return usersWithRoles.Select(x => new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(x.Key),
                Roles = x.Value
            }).ToList();
        }

        public async Task<UserWithRoles> GetById(string id)
        {
            var user = await _identityUser.GetByIdAsync(id);
            var role = await _identityUser.GetUserRole(id);
            return new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(user),
                Roles = role.ToList()
            }; 
        }
    }
}
