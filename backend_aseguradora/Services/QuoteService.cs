using backend_aseguradora.Data;
using backend_aseguradora.Models;
using backend_aseguradora.Utils;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend_aseguradora.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly ApplicationDbContext _context;

        public QuoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextProvisionalQuoteIdAsync()
        {
            // Obtiene el último ID de la cotización (solo para referencia)
            var lastQuote = await _context.Quotes
                .OrderByDescending(q => q.Id)
                .FirstOrDefaultAsync();

            return lastQuote != null ? lastQuote.Id + 1 : 1;
        }

        public async Task<Quote> SaveQuoteAsync(Quote quote, int userId)
        {
            quote.UserId = userId;
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<bool> DeleteQuoteAsync(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return false;
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Quote>> GetQuotesByUserIdAsync(int userId)
        {
            return await _context.Quotes
                .Where(q => q.UserId == userId)
                .Include(q => q.Car)
                .Include(q => q.InsuranceType)
                .Include(q => q.Coverage)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
        {
            return await _context.Quotes
                .Include(q => q.Car)
                .Include(q => q.InsuranceType)
                .Include(q => q.Coverage)
                .Include(q => q.User)
                .ThenInclude(u => u.Person)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}
