using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> HasRelatedDataAsync(int id)
        {
            // Ignora filtros de tenant: el SuperAdmin no pertenece a ninguna empresa
            return await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.IdCompany == id)
                || await _context.Workdays.IgnoreQueryFilters().AnyAsync(w => w.IdCompany == id)
                || await _context.Liquidations.IgnoreQueryFilters().AnyAsync(l => l.IdCompany == id)
                || await _context.Details.IgnoreQueryFilters().AnyAsync(d => d.IdCompany == id);
        }
    }
}
