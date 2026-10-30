using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IWorkdayRepository : IBaseRepository<Workday>
    {
        Task<List<Workday>> GetAllByCompanyAsync(int companyId);
        Task<List<Workday>> GetByUserAsync(int userId);
        Task<List<Workday>> GetPendingByCompanyAsync(int companyId);
        Task<List<Workday>> GetResolvedByCompanyAsync(int companyId);
        Task<List<Workday>> GetByCompanyAndPeriodAsync(int companyId, DateTime fromInclusive, DateTime toExclusive);
    }
}
