using PlataformaAutogestion.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface ILiquidationRepository : IBaseRepository<Liquidation>
    {
        Task<List<Liquidation>> GetAllByCompanyAsync(int companyId);
        Task<bool> ExistsLiquidationForPeriodAsync(int companyId, int month, int year);
        Task<Liquidation?> GetByPeriodAsync(int companyId, int month, int year);

    }
}