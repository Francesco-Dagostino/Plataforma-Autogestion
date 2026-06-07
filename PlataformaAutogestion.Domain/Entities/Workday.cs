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
        public DateTime DateEntry { get; set; }
        public DateTime? DateApproval { get; set; } // nullable: se completa cuando se aprueba
        public StatusDay Estado { get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }
        public int IdUser { get; set; }
        public User Usuario { get; set; }
        public Workday() { }
    }
}