using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;

namespace PlataformaAutogestion.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportsController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("recibos/{id}")]
        public async Task<IActionResult> GenerarRecibos(int id)
        {
            var pdf = await _reporteService.GenerarPdfRecibosAsync(id);

            return File(
                pdf,
                "application/pdf",
                $"recibos_{id}.pdf"
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("banco/{id}")]
        public async Task<IActionResult> GenerarLoteBanco(int id)
        {
            var txt = await _reporteService.GenerarTxtBancoAsync(id);

            return File(
                txt,
                "text/plain",
                $"lote_banco_{id}.txt"
            );
        }
    }
}