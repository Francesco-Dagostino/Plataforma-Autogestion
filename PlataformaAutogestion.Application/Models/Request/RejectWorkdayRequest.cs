using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class RejectWorkdayRequest
    {
        [Required(ErrorMessage = "El motivo de rechazo es obligatorio.")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 250 caracteres.")]
        public string MotivoRechazo { get; set; } = string.Empty;
    }
}
