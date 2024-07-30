using backend_aseguradora.Models;

namespace backend_aseguradora.Services
{
    public interface IQuoteService
    {
        Task<int> GetNextProvisionalQuoteIdAsync();
        Task<Quote> SaveQuoteAsync(Quote quote, int userId);
        Task<bool> DeleteQuoteAsync(int id);
        Task<IEnumerable<Quote>> GetQuotesByUserIdAsync(int userId);
        Task<IEnumerable<Quote>> GetAllQuotesAsync();
    }
}
