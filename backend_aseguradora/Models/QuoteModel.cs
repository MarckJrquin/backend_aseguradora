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

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Cost { get; set; }

        public int InsuranceTypeId { get; set; } // "terceros", "completo"
        public int CoverageId { get; set; } // "responsabilidad civil", "limitada", "amplia"
    }
}
