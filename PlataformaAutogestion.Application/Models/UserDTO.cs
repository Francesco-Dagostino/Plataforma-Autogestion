using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public Roles Role { get; set; }
        public int IdCompany { get; set; }

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
