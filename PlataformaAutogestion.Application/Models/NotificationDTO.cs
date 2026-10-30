using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Application.Models
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? IdWorkday { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public static NotificationDTO FromEntity(Notification n) => new NotificationDTO
        {
            Id = n.Id,
            Type = n.Type,
            Message = n.Message,
            IdWorkday = n.IdWorkday,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        };
    }

    public class NotificationListResponse
    {
        public int UnreadCount { get; set; }
        public List<NotificationDTO> Items { get; set; } = new();
    }
}
