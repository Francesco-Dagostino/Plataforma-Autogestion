using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Requests
{
    public class CompanyDailyLimitRequest
    {
        [Required]
        [Range(0.5, 24, ErrorMessage = "El tope diario debe estar entre 0,5 y 24 horas.")]
        public decimal DailyHoursLimit { get; set; }
    }
}
