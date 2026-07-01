using System.ComponentModel.DataAnnotations;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class UserUpdateRoleRequest
    {
        [Required]
        public Roles Role { get; set; }
    }
}