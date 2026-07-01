using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Liquidation
    {
        public int Id { get; set; }

        [Required]
        public DateTime LiquidationDate { get; set; }

        [Required]
        public DateTime ExecutionDate { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999")]
        public decimal Total { get; set; }

        [Required]
        public bool IsClosed { get; set; }

        [Required]
        public int IdCompany { get; set; }

        public Company Company { get; set; } = null!;

        public List<DetailLiquidation> detailLiquidations { get; set; } = new();

        public Liquidation() { }
    }
}