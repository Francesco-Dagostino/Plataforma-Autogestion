using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlataformaAutogestion.Application.DTOs
{
    public class HolidayDTO
    {
        [Required]
        [JsonPropertyName("fecha")]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("tipo")]
        public string Type { get; set; } = string.Empty;
    }
}