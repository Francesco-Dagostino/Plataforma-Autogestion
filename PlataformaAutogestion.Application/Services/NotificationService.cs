using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using System.Globalization;

namespace PlataformaAutogestion.Application.Services
{
    public class NotificationService : INotificationService
    {
        private const int MaxItems = 30;
        public const string TypeWorkdayLoaded = "WorkdayLoaded";
        public const string TypeWorkdayEdited = "WorkdayEdited";

        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        // Avisa al Admin de la empresa que un empleado cargó (o modificó) una jornada pendiente.
        public async Task NotifyWorkdayLoadedAsync(Workday workday, string userName, bool edited)
        {
            var hours = workday.HoursWorked.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',');
            var day = workday.DateEntry.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var message = edited
                ? $"{userName} modificó su jornada del {day} ({hours} h)."
                : $"{userName} cargó {hours} h del {day}.";

            await _repository.AddAsync(new Notification
            {
                IdCompany = workday.IdCompany,
                IdUser = workday.IdUser,
                IdWorkday = workday.Id,
                Type = edited ? TypeWorkdayEdited : TypeWorkdayLoaded,
                Message = message.Length > 250 ? message[..250] : message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }

        public async Task<NotificationListResponse> GetByCompanyAsync(int companyId)
        {
            var items = await _repository.GetLatestByCompanyAsync(companyId, MaxItems);
            var unread = await _repository.CountUnreadByCompanyAsync(companyId);

            return new NotificationListResponse
            {
                UnreadCount = unread,
                Items = items.Select(NotificationDTO.FromEntity).ToList()
            };
        }

        public Task MarkAsReadAsync(int companyId, int id) => _repository.MarkAsReadAsync(companyId, id);

        public Task MarkAllAsReadAsync(int companyId) => _repository.MarkAllAsReadAsync(companyId);

        // Al aprobar o rechazar una jornada, su aviso deja de estar pendiente de atención.
        public Task MarkWorkdayHandledAsync(int companyId, string workdayId) => _repository.MarkWorkdayAsReadAsync(companyId, workdayId);
    }
}
