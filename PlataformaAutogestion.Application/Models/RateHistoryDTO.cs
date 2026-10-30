using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Application.Models
{
    public class RateHistoryDTO
    {
        public int Id { get; set; }
        public string RateType { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public decimal? PreviousValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? UserName { get; set; }

        public static RateHistoryDTO FromEntity(RateHistory r) => new RateHistoryDTO
        {
            Id = r.Id,
            RateType = r.RateType,
            Value = r.Value,
            PreviousValue = r.PreviousValue,
            ChangedAt = r.ChangedAt,
            UserName = r.UserName
        };
    }
}
