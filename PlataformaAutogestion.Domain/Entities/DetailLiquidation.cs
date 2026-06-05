using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class DetailLiquidation
    {
        public int Id { get; set; }
        public int TotalHours { get; set; }
        public int amount  { get; set; }
        public int IdLiquidation { get; set; }
        public Liquidation Liquidation { get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }
        public int IdUser {  get; set; }
        public User User { get; set; }

        public DetailLiquidation() { }
    }
}
