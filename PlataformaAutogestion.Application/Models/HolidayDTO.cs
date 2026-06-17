using System.Text.Json.Serialization;

namespace PlataformaAutogestion.Application.DTOs
{
    public class HolidayDTO
    {
        [JsonPropertyName("fecha")]
        public DateTime Date { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public string Type { get; set; } = string.Empty;
    }
}