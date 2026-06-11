using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllByCompanyAsync(int companyId)
        {
            return await _context.Users
                .Where(u => u.IdCompany == companyId)
                .ToListAsync();
        }

        public async Task<User?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.workdays)
                .Include(u => u.detailLiquidations)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

    }
}
