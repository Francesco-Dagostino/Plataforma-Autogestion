using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using System.Security.Claims;

namespace PlataformaAutogestion.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("superadmin/{companyId:int}/recibos/{id:int}")]
        public async Task<IActionResult> GenerarRecibosSuperAdmin(int companyId, int id)
        {
            var pdf = await _reporteService.GenerarPdfRecibosEmpresaAsync(id, companyId);
            return File(pdf, "application/pdf", $"recibos_empresa_{companyId}_{id}.pdf");
        }

        [Authorize(Roles = "Empleado")]
        [HttpGet("mi-recibo/{detailId:int}")]
        public async Task<IActionResult> GenerarMiRecibo(int detailId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var pdf = await _reporteService.GenerarPdfReciboEmpleadoAsync(detailId, userId);

            return File(pdf, "application/pdf", $"recibo_{detailId}.pdf");
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("superadmin/{companyId:int}/banco/{id:int}")]
        public async Task<IActionResult> GenerarLoteBancoSuperAdmin(int companyId, int id)
        {
            var txt = await _reporteService.GenerarTxtBancoEmpresaAsync(id, companyId);
            return File(txt, "text/plain", $"lote_banco_empresa_{companyId}_{id}.txt");
        }
    }
}
