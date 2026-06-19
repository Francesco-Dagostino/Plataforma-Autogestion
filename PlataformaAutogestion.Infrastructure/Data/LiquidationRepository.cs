using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class LiquidationRepository : BaseRepository<Liquidation>, ILiquidationRepository
    {
        public readonly ApplicationDbContext _context;
        public LiquidationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Liquidation>> GetAllByCompanyAsync(int companyId)
        {
            return await _context.Set<Liquidation>()
                 .Include(l => l.detailLiquidations)
                 .Where(l => l.IdCompany == companyId)
                 .ToListAsync();
        }

        public async Task<bool> ExistsLiquidationForPeriodAsync(int companyId, int month, int year)
        {
            return await _context.Set<Liquidation>()
                .AnyAsync(l => l.IdCompany == companyId
                            && l.LiquidationDate.Month == month
                            && l.LiquidationDate.Year == year);
        }

        public async Task<Liquidation?> GetByPeriodAsync(int companyId, int month, int year)
        {
            return await _context.Set<Liquidation>()
                .Include(l => l.detailLiquidations)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(l =>
                    l.IdCompany == companyId &&
                    l.LiquidationDate.Month == month &&
                    l.LiquidationDate.Year == year &&
                    l.IsClosed == true);
        }

        public async Task<Liquidation?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Set<Liquidation>()
                .Include(l => l.detailLiquidations)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}