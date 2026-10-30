using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/estadisticas")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        // Horas, costo y rendimiento por empleado de un mes. Sin parámetros usa el mes actual.
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetMonthly([FromQuery] int? month, [FromQuery] int? year)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var today = DateTime.UtcNow;
            var result = await _statisticsService.GetMonthlyAsync(companyId, month ?? today.Month, year ?? today.Year);
            return Ok(result);
        }
    }
}
