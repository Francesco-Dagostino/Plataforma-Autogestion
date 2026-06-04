using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Liquidation
    {
        public int IdLiquidation {  get; set; }
        public DateTime LiquidationDate { get; set; }
        public int Total {  get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }

        public List<DetailLiquidation> detailLiquidations { get; set; } = new List<DetailLiquidation>();

        public Liquidation() { }
    }
}
