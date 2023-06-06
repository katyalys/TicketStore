using Identity.Application.Dtos;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<int> RegisterCustomer(RegisterUser registerUser);
        Task<bool> DeleteUser(string id);
        Task<UserWithRoles> ChangeRole(string id, string role);
        Task<List<UserWithRoles>> GetAllUsers();
        Task<UserWithRoles> GetById(string id);
    }
}
