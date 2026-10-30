using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Requests;
using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyDTO> CreateAsync(CompanyCreateRequest dto);
        Task<List<CompanyDTO>> GetAllAsync();
        Task<CompanyDTO> GetByIdAsync(int id);
        Task UpdateAsync(int id, CompanyCreateRequest dto);
        Task UpdateProfileAsync(int id, CompanyProfileRequest dto);
        Task UpdateHourlyRateAsync(int id, CompanyHourlyRateRequest dto, int? userId, string? userName);
        Task UpdateDailyLimitAsync(int id, CompanyDailyLimitRequest dto);
        Task UpdateHolidayRateAsync(int id, CompanyHolidayRateRequest dto, int? userId, string? userName);
        Task<List<RateHistoryDTO>> GetRateHistoryAsync(int companyId);
        Task DeleteAsync(int id);
    }
}
