using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain.ErrorModels;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Identity.Application.Services;

namespace Identity.Application.Services
{
    public class UserService: IUserService
    {

        private readonly IMapper _mapper;
        private readonly IUserStorageProvider _identityUser;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IValidator<RegisterUser> _validator;
        public UserService(IUserStorageProvider identityUser, IMapper mapper, RoleManager<IdentityRole> roleManager,
            IValidator<RegisterUser> validator)
        {
            _identityUser = identityUser;
            _mapper = mapper;
            _roleManager = roleManager;
            _validator = validator;
        }

        public async Task<Result<string>> RegisterCustomer(RegisterUser registerUser)
        {
            ValidationResult result = await _validator.ValidateAsync(registerUser);
            if (!result.IsValid)
            {
                return ResultReturnService.CreateErrorResult<string>(ErrorStatusCode.WrongAction, "Invalid values");
            }

            var user = _mapper.Map<IdentityUser>(registerUser);
            var isExist = await _identityUser.CheckIfExists(user);

            if (isExist)
            {
                return ResultReturnService.CreateErrorResult<string>(ErrorStatusCode.WrongAction, "User already registered");
            };

            try
            {
                var resultList = await _identityUser.AddAsync(user, registerUser.Password);
                if (resultList.Any(res => !res.Succeeded))
                {
                    return ResultReturnService.CreateErrorResult<string>(ErrorStatusCode.WrongAction, "Something went wrong");
                }
            }
            catch
            {
                return ResultReturnService.CreateErrorResult<string>(ErrorStatusCode.WrongAction, "Length of username max 15 symblos");
            }

            await _identityUser.AddClaimssAsync(user);

            return new Result<string>()
            {
                Value = "Success",
            };
        }

        public async Task<Result<string>> DeleteUser(string id) 
        {
            var user = await _identityUser.GetByIdAsync(id);

            if (user == null) 
            {
                return ResultReturnService.CreateErrorResult<string>(ErrorStatusCode.NotFound, "There is no user with such id found");
            }
            await _identityUser.DeleteAsync(user);

            return new Result<string>(){ Value = "Success"};
        }

        public async Task<Result<UserWithRoles>> ChangeRole(string id, string role)
        {
            if (! await _roleManager.RoleExistsAsync(role))
            {
                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "No such role");
            }

            var user = await _identityUser.GetByIdAsync(id);
            if (user == null)
            {
                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "There is no user with such id found");
            }
            var updatedUser = await _identityUser.UpdateUserRoleAsync(user, role);
            var userWithRole = new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(updatedUser.user),
                Roles = new List<string>() { updatedUser.newRole }
            };

            return new Result<UserWithRoles>() { Value = userWithRole };
        }

        public async Task<Result<List<UserWithRoles>>> GetAllUsers()
        {
            var usersWithRoles = await _identityUser.GetAllUsersWithRolesAsync();

            if (usersWithRoles == null)
            {
                return ResultReturnService.CreateErrorResult<List<UserWithRoles>>(ErrorStatusCode.NotFound, "No users");
            };

            return new Result<List<UserWithRoles>>()
            {
                Value = usersWithRoles
            };
        }

        public async Task<Result<UserWithRoles>> GetById(string id)
        {
            var user = await _identityUser.GetByIdAsync(id);
            if (user == null)
            {
                return ResultReturnService.CreateErrorResult<UserWithRoles>(ErrorStatusCode.NotFound, "There is no user with such id found");
            }

            var role = await _identityUser.GetUserRole(id);
          
            var userWithRole = new UserWithRoles
            {
                User = _mapper.Map<UserViewModel>(user),
                Roles = role.ToList()
            };

            return new Result<UserWithRoles>()
            {
                Value = userWithRole
            };
        }
    }
}
