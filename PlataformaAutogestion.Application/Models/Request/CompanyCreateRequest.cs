using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Requests
{
    public class CompanyCreateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Range(10000000000L, 99999999999L, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public long Cuit { get; set; }
        [Required]
        public DateTime DateHigh { get; set; }
        [Required]
        public int ParameterSystem { get; set; }
    }
}