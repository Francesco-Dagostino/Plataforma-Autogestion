using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities
{
    // Registro de cada cambio en los valores hora de una empresa, para saber qué se pagó y desde cuándo.
    public class RateHistory
    {
        public int Id { get; set; }

        [Required]
        public int IdCompany { get; set; }

        public Company Company { get; set; } = null!;

        // "Normal" (valor hora) o "Feriado" (valor hora de feriados y fines de semana).
        [Required]
        [MaxLength(20)]
        public string RateType { get; set; } = string.Empty;

        // Monto por hora que pasó a pagarse (en "Feriado" es el monto efectivo, aunque se calcule como el doble).
        public decimal? Value { get; set; }

        // Monto por hora que se pagaba antes; null si es el primer valor registrado.
        public decimal? PreviousValue { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; }

        public int? IdUser { get; set; }

        // Usuario que hizo el cambio, guardado como texto para conservarlo aunque el usuario se dé de baja.
        [MaxLength(100)]
        public string? UserName { get; set; }
    }
}
