using System.ComponentModel.DataAnnotations;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime CreationDate { get; set; }

    [Required]
    public Roles role { get; set; }

    public int? IdCompany { get; set; }

    public Company? Company { get; set; }

    public List<DetailLiquidation> detailLiquidations { get; set; } = new();
    public List<Workday> workdays { get; set; } = new();

    public User() { }
}