using PlataformaAutogestion.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models
{
    public class UserDTO
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
        public DateTime CreationDate { get; set; }

        [Required]
        public Roles Role { get; set; }

        public int? IdCompany { get; set; }

        public static UserDTO FromEntity(User user) => new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserName = user.UserName,
            CreationDate = user.CreationDate,
            Role = user.role,
            IdCompany = user.IdCompany
        };
    }
}