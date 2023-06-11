using Identity.Application.Dtos;
using Identity.Domain.ErrorModels;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<string>> RegisterCustomer(RegisterUser registerUser);
        Task<Result<string>> DeleteUser(string id);
        Task<Result<UserWithRoles>> ChangeRole(string id, string role);
        Task<Result<List<UserWithRoles>>> GetAllUsers();
        Task<Result<UserWithRoles>> GetById(string id);
    }
}
