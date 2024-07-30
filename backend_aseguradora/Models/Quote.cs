using System.ComponentModel.DataAnnotations;

namespace backend_aseguradora.Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int InsuranceTypeId { get; set; }
        public InsuranceType InsuranceType { get; set; }

        [Required]
        public int CoverageId { get; set; }
        public Coverage Coverage { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
