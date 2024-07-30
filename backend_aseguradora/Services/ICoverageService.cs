using backend_aseguradora.Models;

namespace backend_aseguradora.Services
{
    public interface ICoverageService
    {
        Task<IEnumerable<Coverage>> GetCoveragesAsync();
        Task<Coverage> GetCoverageByIdAsync(int id);
    }
}
