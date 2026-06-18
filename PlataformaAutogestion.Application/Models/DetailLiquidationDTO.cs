using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Models
{
    public class DetailLiquidationDTO
    {
        public int Id { get; set; }
        public decimal TotalHours { get; set; }
        public decimal Amount { get; set; }
        public int IdLiquidation { get; set; }
        public int IdUser { get; set; }
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
