using PlataformaAutogestion.Application.Models.Response;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsResponse> GetMonthlyAsync(int companyId, int month, int year);
    }
}
