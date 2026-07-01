using System.ComponentModel.DataAnnotations;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Workday
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
        public int IdCompany { get; set; }

        public Company Company { get; set; } = null!;

        [Required]
        public int IdUser { get; set; }

        public User Usuario { get; set; } = null!;

        public Workday() { }
    }
}