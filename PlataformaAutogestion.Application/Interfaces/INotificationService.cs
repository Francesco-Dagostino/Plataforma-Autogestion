using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyWorkdayLoadedAsync(Workday workday, string userName, bool edited);
        Task<NotificationListResponse> GetByCompanyAsync(int companyId);
        Task MarkAsReadAsync(int companyId, int id);
        Task MarkAllAsReadAsync(int companyId);
        Task MarkWorkdayHandledAsync(int companyId, string workdayId);
    }
}
