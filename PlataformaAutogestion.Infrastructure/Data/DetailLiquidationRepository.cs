using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class DetailLiquidationRepository : BaseRepository<DetailLiquidation>, IDetailLiquidationRepositoy
    {
        private readonly ApplicationDbContext _context;
        public DetailLiquidationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DetailLiquidation>> GetByUserAsync(int userId)
        {
            return await _context.Details
                .Where(d => d.IdUser == userId)
                .ToListAsync();
        }

        public async Task<List<DetailLiquidation>> GetByLiquidationAsync(int idLiquidation, int idCompany)
        {
            return await _context.Details
                .Where(d => d.IdLiquidation == idLiquidation && d.IdCompany == idCompany)
                .ToListAsync();
        }
    }
}
