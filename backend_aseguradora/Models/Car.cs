using System.ComponentModel.DataAnnotations;

namespace backend_aseguradora.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [MaxLength(75)]
        public string Brand { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid cost")]
        public decimal Cost { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }
    }
}
