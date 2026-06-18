using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Models.Response
{
    public class DetalleLiquidacionResponse
    {
        public int IdUser { get; set; }
        public string NombreEmpleado { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal Monto { get; set; }
    }
}
