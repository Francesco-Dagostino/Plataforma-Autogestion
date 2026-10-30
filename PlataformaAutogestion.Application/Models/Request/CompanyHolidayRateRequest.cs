using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Requests
{
    public class CompanyHolidayRateRequest
    {
        // null = usar el doble del valor hora normal.
        [Range(0.01, 999999, ErrorMessage = "El valor hora de feriado debe ser mayor a 0.")]
        public decimal? HolidayHourlyRate { get; set; }
    }
}
