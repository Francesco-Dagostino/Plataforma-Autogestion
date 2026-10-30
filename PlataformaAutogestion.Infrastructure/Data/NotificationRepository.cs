using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetLatestByCompanyAsync(int companyId, int take)
        {
            return await _context.Notifications
                .Where(n => n.IdCompany == companyId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountUnreadByCompanyAsync(int companyId)
        {
            return await _context.Notifications
                .CountAsync(n => n.IdCompany == companyId && !n.IsRead);
        }

        public async Task MarkAllAsReadAsync(int companyId)
        {
            await _context.Notifications
                .Where(n => n.IdCompany == companyId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task MarkAsReadAsync(int companyId, int id)
        {
            await _context.Notifications
                .Where(n => n.IdCompany == companyId && n.Id == id && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task MarkWorkdayAsReadAsync(int companyId, string workdayId)
        {
            await _context.Notifications
                .Where(n => n.IdCompany == companyId && n.IdWorkday == workdayId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}
