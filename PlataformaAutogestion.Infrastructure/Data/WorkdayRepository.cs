using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class WorkdayRepository : BaseRepository<Workday>, IWorkdayRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkdayRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Workday>> GetAllByCompanyAsync(int companyId)
        {
            return await _context.Workdays
                .Where(w => w.IdCompany == companyId)
                .ToListAsync();
        }

        public async Task<List<Workday>> GetByUserAsync(int userId)
        {
            return await _context.Workdays
                .Where(w => w.IdUser == userId)
                .ToListAsync();
        }

        public async Task<List<Workday>> GetPendingByCompanyAsync(int companyId)
        {
            return await _context.Workdays
                .Include(w => w.Usuario)
                .Where(w => w.IdCompany == companyId && w.Estado == StatusDay.Pendiente)
                .ToListAsync();
        }

        public async Task<Workday?> GetByDateAndUserAsync(DateTime date, int userId)
        {
            return await _context.Workdays
                .FirstOrDefaultAsync(w => w.IdUser == userId && w.DateEntry.Date == date.Date);
        }
    }
}
