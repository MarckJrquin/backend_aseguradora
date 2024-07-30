using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend_aseguradora.Models
{
    public class QuoteModel
    {
        public int Year { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double Cost { get; set; }
        public int InsuranceTypeId { get; set; } // "terceros", "completo"
        public int CoverageId { get; set; } // "responsabilidad civil", "limitada", "amplia"
    }
}
