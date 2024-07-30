using backend_aseguradora.Data;
using backend_aseguradora.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_aseguradora.Services
{
    public class CoverageService : ICoverageService
    {
        private readonly ApplicationDbContext _context;

        public CoverageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coverage>> GetCoveragesAsync()
        {
            return await _context.Coverages.ToListAsync();
        }

        public async Task<Coverage> GetCoverageByIdAsync(int id)
        {
            return await _context.Coverages.FindAsync(id);
        }
    }
}
