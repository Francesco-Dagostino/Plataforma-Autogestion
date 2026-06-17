using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using System.Globalization;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        // GET: api/holidays/2024
        [HttpGet("{year:int}")]
        public async Task<IActionResult> GetHolidaysByYear(int year)
        {
            var holidays = await _holidayService.GetHolidaysByYearAsync(year);

            if (!holidays.Any())
            {
                return NotFound(new
                {
                    Message = $"No se encontraron feriados para el año {year}."
                });
            }

            return Ok(holidays);
        }

        // GET: api/holidays/check?date=2024-05-01
        [HttpGet("check")]
        public async Task<IActionResult> CheckIfHoliday([FromQuery] string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return BadRequest(new
                {
                    Error = "Fecha requerida"
                });
            }

            if (!DateTime.TryParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return BadRequest(new
                {
                    Error = "Formato inválido",
                    Message = "Debe usar YYYY-MM-DD"
                });
            }

            var isHoliday = await _holidayService.IsHolidayAsync(parsedDate);

            return Ok(new
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                IsHoliday = isHoliday
            });
        }
    }
}