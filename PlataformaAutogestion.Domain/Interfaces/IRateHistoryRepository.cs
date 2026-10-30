using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IRateHistoryRepository : IBaseRepository<RateHistory>
    {
        Task<List<RateHistory>> GetLatestByCompanyAsync(int companyId, int take);
    }
}
