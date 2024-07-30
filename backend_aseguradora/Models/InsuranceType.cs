using System.ComponentModel.DataAnnotations;

namespace backend_aseguradora.Models
{
    public class InsuranceType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
