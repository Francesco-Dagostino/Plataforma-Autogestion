using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class MfaVerifyRequest
    {
        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
        public string Code { get; set; } = string.Empty;
    }
}
