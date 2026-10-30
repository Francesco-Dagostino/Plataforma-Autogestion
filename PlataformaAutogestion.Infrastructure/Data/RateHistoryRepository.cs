using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class RateHistoryRepository : BaseRepository<RateHistory>, IRateHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public RateHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RateHistory>> GetLatestByCompanyAsync(int companyId, int take)
        {
            return await _context.RateHistories
                .Where(r => r.IdCompany == companyId)
                .OrderByDescending(r => r.ChangedAt)
                .ThenByDescending(r => r.Id)
                .Take(take)
                .ToListAsync();
        }
    }
}
