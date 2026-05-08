using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Liquidacion
    {
        public int IdLiquidacion {  get; set; }
        public DateTime FechaLiquidacion { get; set; }
        public int Total {  get; set; }
        public int IdEmpresa { get; set; }
        public Empresa Empresa { get; set; }

        public List<DetalleLiquidacion> detalleLiquidacions { get; set; } = new List<DetalleLiquidacion>();

        public Liquidacion() { }
    }
}
