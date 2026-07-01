using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}