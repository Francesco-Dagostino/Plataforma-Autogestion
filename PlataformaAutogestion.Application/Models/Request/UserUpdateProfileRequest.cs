using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class UserUpdateProfileRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MinLength(6)]
        [MaxLength(100)]
        public string? Password { get; set; }
    }
}