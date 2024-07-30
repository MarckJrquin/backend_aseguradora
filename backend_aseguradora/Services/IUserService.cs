using backend_aseguradora.Models;

namespace backend_aseguradora.Services
{
    public interface IUserService
    {
        Task<object> GetUserByIdAsync(int id);
        Task<string> EditUserAsync(int id, EditUserModel model);
        Task<string> UpdatePasswordAsync(int id, UpdatePasswordModel model);
        Task<string> DeleteUserAsync(int id);
        Task<IEnumerable<object>> GetAllUsersAsync();
    }
}
