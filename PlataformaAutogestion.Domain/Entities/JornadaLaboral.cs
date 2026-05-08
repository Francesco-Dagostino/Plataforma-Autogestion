using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities
{
    public class JornadaLaboral
    {
        public string IdJornada { get; set; }
        public int HorasTrabajadas { get; set; }
        public DateTime FechaIngreso { get; set; } // esto es para la fecha en que se cargo la jornada!
        public DateTime FechaAprobacion { get; set; } // y esto para cuando fue aprobada!
        public EstadoJornada Estado { get; set; }
        public int IdEmpresa { get; set; }
        public Empresa Empresa { get; set; }
        public int Id {  get; set; }
        public Usuario Usuario { get; set; }

        public JornadaLaboral() { }

    }
}
