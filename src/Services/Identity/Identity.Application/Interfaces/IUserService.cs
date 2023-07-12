using Identity.Application.Dtos;
using Identity.Domain.ErrorModels;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result> RegisterCustomerAsync(RegisterUser registerUser);
        Task<Result> DeleteUserAsync(string id);
        Task<Result<UserWithRoles>> ChangeRoleAsync(string id, string role);
        Task<Result<List<UserWithRoles>>> GetAllUsersAsync();
        Task<Result<UserWithRoles>> GetByIdAsync(string id);
    }
}
