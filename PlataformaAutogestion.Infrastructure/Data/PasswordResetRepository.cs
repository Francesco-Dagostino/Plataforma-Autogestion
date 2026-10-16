using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.PasswordResetTokens
                .IgnoreQueryFilters()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.Used);
        }

        public async Task UpdateAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        // Invalida tokens anteriores del mismo usuario
        public async Task InvalidateAllForUserAsync(int userId)
        {
            var tokens = await _context.PasswordResetTokens
                .IgnoreQueryFilters()
                .Where(t => t.UserId == userId && !t.Used)
                .ToListAsync();

            foreach (var token in tokens)
                token.Used = true;

            await _context.SaveChangesAsync();
        }
    }
}
