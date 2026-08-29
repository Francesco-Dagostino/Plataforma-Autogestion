using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IWorkdayService
    {
        Task<List<WorkdayDTO>> GetByUserAsync(int userId);
        Task<List<WorkdayDTO>> GetPendingByCompanyAsync(int companyId);
        Task<WorkdayDTO> AddAsync(WorkdayCreateRequest request, int userId);
        Task ApproveAsync(string id, int companyId);
        Task RejectAsync(string id, int companyId);
    }
}
