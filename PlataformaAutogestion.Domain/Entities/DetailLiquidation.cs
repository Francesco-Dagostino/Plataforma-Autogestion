using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities
{
    public class DetailLiquidation
    {
        public int Id { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999")]
        public decimal TotalHours { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Required]
        public int IdLiquidation { get; set; }

        public Liquidation Liquidation { get; set; } = null!;

        [Required]
        public int IdCompany { get; set; }

        public Company Company { get; set; } = null!;

        [Required]
        public int IdUser { get; set; }

        public User User { get; set; } = null!;

        public DetailLiquidation() { }
    }
}