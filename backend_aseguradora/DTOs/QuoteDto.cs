namespace backend_aseguradora.DTOs
{
    public class QuoteDto
    {
        public int Id { get; set; }
        public CarDto Car { get; set; }
        public int InsuranceTypeId { get; set; }
        public string InsuranceTypeName { get; set; }
        public int CoverageId { get; set; }
        public string CoverageName { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
