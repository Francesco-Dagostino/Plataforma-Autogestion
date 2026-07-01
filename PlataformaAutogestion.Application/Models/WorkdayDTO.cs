using PlataformaAutogestion.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models
{
    public class WorkdayDTO
    {
        [Required]
        [MaxLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "24")]
        public decimal HoursWorked { get; set; }

        [Required]
        public DateTime DateEntry { get; set; }

        public DateTime? DateApproval { get; set; }

        [Required]
        public StatusDay Estado { get; set; }

        [Required]
        public int IdUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public int IdCompany { get; set; }

        public static WorkdayDTO FromEntity(Workday w) => new WorkdayDTO
        {
            Id = w.Id,
            HoursWorked = w.HoursWorked,
            DateEntry = w.DateEntry,
            DateApproval = w.DateApproval,
            Estado = w.Estado,
            IdUser = w.IdUser,
            UserName = w.Usuario?.Name ?? string.Empty,
            IdCompany = w.IdCompany
        };
    }
}