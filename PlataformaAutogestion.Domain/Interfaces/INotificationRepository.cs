using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<List<Notification>> GetLatestByCompanyAsync(int companyId, int take);
        Task<int> CountUnreadByCompanyAsync(int companyId);
        Task MarkAllAsReadAsync(int companyId);
        Task MarkAsReadAsync(int companyId, int id);
        Task MarkWorkdayAsReadAsync(int companyId, string workdayId);
    }
}
