using PlataformaAutogestion.Application.DTOs;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IHolidayService
    {
        Task<IEnumerable<HolidayDTO>> GetHolidaysByYearAsync(int year);
        Task<bool> IsHolidayAsync(DateTime date);
    }
}