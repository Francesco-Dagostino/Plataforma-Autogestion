using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class WorkdayCreateRequest
    {
        [Required]
        [Range(typeof(decimal), "0", "24", ErrorMessage = "Las horas trabajadas deben estar entre 0 y 24.")]
        public decimal HoursWorked { get; set; } = 0;

        [Required]
        public DateTime DateEntry { get; set; } = DateTime.Today;
    }
}