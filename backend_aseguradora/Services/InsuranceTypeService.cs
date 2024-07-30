using backend_aseguradora.Data;
using backend_aseguradora.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_aseguradora.Services
{
    public class InsuranceTypeService : IInsuranceTypeService
    {
        private readonly ApplicationDbContext _context;

        public InsuranceTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InsuranceType>> GetInsuranceTypesAsync()
        {
            return await _context.InsuranceTypes.ToListAsync();
        }

        public async Task<InsuranceType> GetInsuranceTypeByIdAsync(int id)
        {
            return await _context.InsuranceTypes.FindAsync(id);
        }
    }
}
