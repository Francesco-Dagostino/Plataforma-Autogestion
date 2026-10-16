using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Domain.Entities;

public class PasswordResetToken
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    // Hash SHA256 del token (nunca se guarda el token en texto plano)
    [Required]
    [MaxLength(128)]
    public string TokenHash { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public bool Used { get; set; } = false;

    public User? User { get; set; }
}
