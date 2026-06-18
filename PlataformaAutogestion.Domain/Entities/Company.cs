using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [Range(10000000000L, 99999999999L, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public long Cuit { get; set; }
        public DateTime DateHigh { get; set; }
        public decimal ParameterSystem { get; set; }

        //relaciones
        public List<Liquidation> liquidations { get; set; } = new List<Liquidation>();
        public List<User> users { get; set; } = new List<User>();
        public List<Workday> workdays { get; set; } = new List<Workday>();
        public List<DetailLiquidation> detailLiquidations { get; set; } = new List<DetailLiquidation>();

        public Company() { }
    }
}
