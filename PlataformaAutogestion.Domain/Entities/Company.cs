using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities
{
    public class Company
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

        // Valor hora en feriados y fines de semana. Si es null se paga el doble del valor hora normal.
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal? HolidayHourlyRate { get; set; }

        // Máximo de horas que un empleado puede declarar en un solo día.
        [Required]
        [Range(typeof(decimal), "0.5", "24", ErrorMessage = "El tope diario debe estar entre 0,5 y 24 horas.")]
        public decimal DailyHoursLimit { get; set; } = 12;

        public List<Liquidation> liquidations { get; set; } = new();
        public List<User> users { get; set; } = new();
        public List<Workday> workdays { get; set; } = new();
        public List<DetailLiquidation> detailLiquidations { get; set; } = new();

        public Company() { }
    }
}