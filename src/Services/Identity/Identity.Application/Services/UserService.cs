using AutoMapper;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain.Entites;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _identityUser;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(IUserRepository identityUser, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _identityUser = identityUser;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<Result<string>> RegisterCustomer(RegisterUser registerUser)
        {
            if (registerUser.Password == registerUser.ConfirmPassword)
            {
                var user = _mapper.Map<IdentityUser>(registerUser);
                var isExist = await _identityUser.CheckIfExists(user);

                if (isExist)
                {
                    return new Result<string>()
                    {
                        Errors = new List<ErrorClass>
                        {
                            new(ErrorStatusCode.WrongAction, "User already registered")
                        }
                    };
                };

                await _identityUser.AddAsync(user, registerUser.Password);
                await _identityUser.AddClaimssAsync(user);

                return new Result<string>()
                {
                    Value = "Success",
                };
            }
            else {
                return new Result<string>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.WrongAction, "Passwords don't match")
                    }
                };
            }
        }

        public async Task<Result<string>> DeleteUser(string id) {
            var user = await _identityUser.GetByIdAsync(id);

            if (user != null) {
                await _identityUser.DeleteAsync(user);
            }
            else
            {
                return new Result<string>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.NotFound, "There is no user with such id found")
                    }
                };
            }
            return new Result<string>(){ Value = "Success"};
        }

        public async Task<Result<UserWithRoles>> ChangeRole(string id, string role)
        {
            if (! await _roleManager.RoleExistsAsync(role))
            {
                return new Result<UserWithRoles>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.NotFound, "No such role")
                    }
                };
            }

            var user = await _identityUser.GetByIdAsync(id);
            if (user != null)
            {
                var updatedUser = await _identityUser.UpdateUserRoleAsync(user, role);
                var userWithRole =  new UserWithRoles
                                    {
                                        User = _mapper.Map<UserViewModel>(updatedUser.user),
                                        Roles = new List<string>() { updatedUser.newRole }
                                    };
                return new Result<UserWithRoles>() { Value = userWithRole };
            }
            else
            {
                return new Result<UserWithRoles>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.NotFound, "There is no user with such id found")
                    }
                };
            }
        }

        public async Task<Result<List<UserWithRoles>>> GetAllUsers()
        {
            var usersWithRoles = await _identityUser.GetAllUsersWithRolesAsync();

            if (usersWithRoles == null)
            {
                return new Result<List<UserWithRoles>>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.NotFound, "No users")
                    }
                };
            };

            var users = usersWithRoles.Select(x => new UserWithRoles
                                        {
                                            User = _mapper.Map<UserViewModel>(x.Key),
                                            Roles = x.Value
                                        }).ToList();

            return new Result<List<UserWithRoles>>()
            {
                Value = users
            };
        }

        public async Task<Result<UserWithRoles>> GetById(string id)
        {
            var user = await _identityUser.GetByIdAsync(id);
            var role = await _identityUser.GetUserRole(id);
            if (user == null || role == null)
            {
                return new Result<UserWithRoles>()
                {
                    Errors = new List<ErrorClass>
                    {
                        new(ErrorStatusCode.NotFound, "There is no user with such id found")
                    }
                };
            }

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
