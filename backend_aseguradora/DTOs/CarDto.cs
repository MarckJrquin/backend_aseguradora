using System.ComponentModel.DataAnnotations;

namespace backend_aseguradora.DTOs
{
    public class CarDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Cost { get; set; }
    }
}
