using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Workday
    {
        public string Id { get; set; }
        public int HoursWorked { get; set; }
        public DateTime DateEntry { get; set; } // esto es para la fecha en que se cargo la jornada!
        public DateTime DateApproval { get; set; } // y esto para cuando fue aprobada!
        public StatusDay Estado { get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }
        public int IdUser {  get; set; }
        public User Usuario { get; set; }

        public Workday() { }

    }
}
