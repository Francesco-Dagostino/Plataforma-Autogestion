using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Requests
{
    public class CompanyHourlyRateRequest
    {
        [Required]
        [Range(0.01, 999999)]
        public decimal ParameterSystem { get; set; }
    }
}
