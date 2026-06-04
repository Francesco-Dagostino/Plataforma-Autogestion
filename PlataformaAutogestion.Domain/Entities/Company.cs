using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Company
    {
        public int IdCompany { get; set; }
        public string Name { get; set; }
        public int Cuit {  get; set; }
        public DateTime DateHigh { get; set; }
        public int ParameterSystem { get; set; }

        //relaciones
        public List<Liquidation> liquidations { get; set; } = new List<Liquidation>();
        public List<User> users { get; set; } = new List<User>();
        public List<Workday> workdays { get; set; } = new List<Workday>();
        public List<DetailLiquidation> detailLiquidations { get; set; } = new List<DetailLiquidation>();

        public Company() { }
    }
}
