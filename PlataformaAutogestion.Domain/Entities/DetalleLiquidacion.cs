using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class DetalleLiquidacion
    {
        public int IdDetalle { get; set; }
        public int TotalHoras { get; set; }
        public int Monto { get; set; }

        public int IdLiquidacion { get; set; }
        public Liquidacion Liquidacion { get; set; }
        public int IdEmpresa { get; set; }
        public Empresa Empresa { get; set; }
        public int Id {  get; set; }
        public Usuario Usuario { get; set; }

        public DetalleLiquidacion() { }
    }
}
