using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class UserUpdateProfileRequest
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string? Password { get; set; } 
    }
}
