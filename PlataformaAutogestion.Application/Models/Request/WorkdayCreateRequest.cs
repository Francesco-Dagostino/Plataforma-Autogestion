using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class WorkdayCreateRequest
    {
        [Required]
        public int HoursWorked { get; set; }
        [Required]
        public DateTime DateEntry { get; set; }
    }
}
