using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities
{
    // Aviso interno para el Admin de una empresa (se muestra en el sistema, no se envía por correo).
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int IdCompany { get; set; }

        public Company Company { get; set; } = null!;

        // Empleado que generó el aviso.
        public int? IdUser { get; set; }

        // Jornada a la que hace referencia; permite marcar el aviso como atendido al aprobar o rechazar.
        [MaxLength(50)]
        public string? IdWorkday { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
