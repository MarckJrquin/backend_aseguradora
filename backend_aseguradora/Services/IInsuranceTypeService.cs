using backend_aseguradora.Models;

namespace backend_aseguradora.Services
{
    public interface IInsuranceTypeService
    {
        Task<IEnumerable<InsuranceType>> GetInsuranceTypesAsync();
        Task<InsuranceType> GetInsuranceTypeByIdAsync(int id); 
    }
}
