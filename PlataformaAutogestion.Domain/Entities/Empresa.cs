using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string Name { get; set; }
        public int Cuit {  get; set; }
        public DateTime FechaAlta { get; set; }
        public int ParametroSistema { get; set; }

        //relaciones
        public List<Liquidacion> Liquidaciones { get; set; } = new List<Liquidacion>();
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<JornadaLaboral> JornadaLaborals { get; set; } = new List<JornadaLaboral>();
        public List<DetalleLiquidacion> detalleLiquidacions { get; set; } = new List<DetalleLiquidacion>();

        public Empresa() { }
    }
}
