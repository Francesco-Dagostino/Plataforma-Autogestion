using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash);
        Task UpdateAsync(PasswordResetToken token);
        Task InvalidateAllForUserAsync(int userId);
    }
}
