using AutoMapper;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain.ErrorModels;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserAccessService _identityUser;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserAccessService identityUser, IMapper mapper, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger)
        {
            _identityUser = identityUser;
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Result> RegisterCustomerAsync(RegisterUser registerUser)
        {
            _logger.LogInformation("Registering customer");
            var user = _mapper.Map<IdentityUser>(registerUser);
            var isExist = await _identityUser.CheckIfExistsAsync(user);

            if (isExist)
            {
                _logger.LogWarning("User already registered");

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "User already registered");
            };

            try
            {
                var resultList = await _identityUser.AddAsync(user, registerUser.Password);

                if (resultList.Any(res => !res.Succeeded))
                {
                    _logger.LogError("Failed to register user");

                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Something went wrong");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register user");

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Length of username max 15 symblos");
            }

            await _identityUser.AddClaimssAsync(user);
            _logger.LogInformation("Customer registered successfully");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteUserAsync(string id) 
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var user = await _identityUser.GetByIdAsync(id);

            if (user == null) 
            {
                _logger.LogWarning("User with id {UserId} not found", id);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "There is no user with such id found");
            }

            await _identityUser.DeleteAsync(user);
            _logger.LogInformation("User deleted successfully");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<UserWithRoles>> ChangeRoleAsync(string id, string role)
        {
            _logger.LogInformation("Changing role for user with ID: {UserId}, Role: {Role}", id, role);

            if (! await _roleManager.RoleExistsAsync(role))
            {
                _logger.LogWarning("Role {role} not found", role);

                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "No such role");
            }

            var user = await _identityUser.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);

                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "There is no user with such id found");
            }

            var updatedUser = await _identityUser.UpdateUserRoleAsync(user, role);
            var userWithRole = new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(updatedUser.user),
                Roles = new List<string>() { updatedUser.newRole }
            };
            _logger.LogInformation("Role changed successfully");

            return new Result<UserWithRoles>() { Value = userWithRole };
        }

        public async Task<Result<List<UserWithRoles>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Retrieving all users with roles");
            var usersWithRoles = await _identityUser.GetAllUsersWithRolesAsync();

            if (usersWithRoles == null)
            {
                _logger.LogWarning("No users found");

                return ResultReturnService.CreateErrorResult<List<UserWithRoles>>(ErrorStatusCode.NotFound, "No users");
            };

            _logger.LogInformation("Retrieved all users with roles successfully");

            return new Result<List<UserWithRoles>>()
            {
                Value = usersWithRoles
            };
        }

        public async Task<Result<UserWithRoles>> GetByIdAsync(string id)
        {
            _logger.LogInformation("Retrieving user by ID: {UserId}", id);
            var user = await _identityUser.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with {UserId} not found", id);

                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "There is no user with such id found");
            }

            var role = await _identityUser.GetUserRoleAsync(id);
            var userWithRole = new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(user),
                Roles = role.ToList()
            };
            _logger.LogInformation("Retrieved user by ID successfully");

            return new Result<UserWithRoles>()
            {
                Value = userWithRole
            };
        }
    }
}
