using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models
{
    public class WorkdayDTO
    {
        public string Id { get; set; }
        public int HoursWorked { get; set; }
        public DateTime DateEntry { get; set; }
        public DateTime? DateApproval { get; set; }
        public StatusDay Estado { get; set; }
        public int IdUser { get; set; }
        public int IdCompany { get; set; }

        public static WorkdayDTO FromEntity(Workday w) => new WorkdayDTO
        {
            Id = w.Id,
            HoursWorked = w.HoursWorked,
            DateEntry = w.DateEntry,
            DateApproval = w.DateApproval,
            Estado = w.Estado,
            IdUser = w.IdUser,
            IdCompany = w.IdCompany
        };
    }
}
