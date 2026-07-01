using PlataformaAutogestion.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models
{
    public class DetailLiquidationDTO
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

        [Required]
        public int IdUser { get; set; }

        [Required]
        public int IdCompany { get; set; }

        public static DetailLiquidationDTO FromEntity(DetailLiquidation entity)
        {
            return new DetailLiquidationDTO
            {
                Id = entity.Id,
                TotalHours = entity.TotalHours,
                Amount = entity.Amount,
                IdLiquidation = entity.IdLiquidation,
                IdUser = entity.IdUser,
                IdCompany = entity.IdCompany
            };
        }
    }
}