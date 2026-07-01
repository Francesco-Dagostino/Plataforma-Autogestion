using PlataformaAutogestion.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models
{
    public class CompanyDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(10000000000L, 99999999999L, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public long Cuit { get; set; }

        [Required]
        public DateTime DateHigh { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal ParameterSystem { get; set; }

        public static CompanyDTO FromEntity(Company company)
        {
            return new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Cuit = company.Cuit,
                DateHigh = company.DateHigh,
                ParameterSystem = company.ParameterSystem
            };
        }
    }
}