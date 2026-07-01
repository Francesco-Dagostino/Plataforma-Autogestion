using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class UserCreateRequest
    {
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
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Roles Role { get; set; }

        public int? IdCompany { get; set; }
    }
}