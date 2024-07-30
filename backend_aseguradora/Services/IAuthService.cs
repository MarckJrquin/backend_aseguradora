using backend_aseguradora.Models;

namespace backend_aseguradora.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(User user, Person person, string role);
        Task<(string token, string role)> LoginAsync(string username, string password);
        string RefreshToken(string token);
    }
}
