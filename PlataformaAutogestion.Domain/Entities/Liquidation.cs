using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Liquidation
    {
        public int Id { get; set; }
        public DateTime LiquidationDate { get; set; } // Ej: 30 de junio (Período)

        // NUEVO: Fecha y hora exacta en la que el Admin generó el cierre
        public DateTime ExecutionDate { get; set; }

        public decimal Total { get; set; }
        public bool IsClosed { get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }

        public List<DetailLiquidation> detailLiquidations { get; set; } = new List<DetailLiquidation>();

        public Liquidation() { }
    }
}
