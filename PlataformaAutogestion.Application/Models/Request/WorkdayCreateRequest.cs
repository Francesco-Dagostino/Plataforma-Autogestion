using System.ComponentModel.DataAnnotations;

namespace PlataformaAutogestion.Application.Models.Request
{
    public class WorkdayCreateRequest
    {
        [Required]
        [Range(1, 24)]
        public int HoursWorked { get; set; }

        [Required]
        public DateTime DateEntry { get; set; }
    }
}