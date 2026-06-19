using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Request;
using System;
using System.Threading.Tasks;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LiquidationController : ControllerBase
    {
        private readonly ILiquidationService _liquidationService;

        public LiquidationController(ILiquidationService liquidationService)
        {
            _liquidationService = liquidationService;
        }

        private int GetCompanyIdFromToken()
        {
            var claim = User.FindFirst("IdCompany")?.Value;
            if (!int.TryParse(claim, out var companyId))
                throw new UnauthorizedAccessException("El token no contiene una empresa válida.");
            return companyId;
        }

        [HttpGet("empleado/{userId}/simular")]
        public async Task<IActionResult> SimularSueldoEmpleado(int userId, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var simulacion = await _liquidationService.SimularSueldoEmpleadoAsync(userId, month, year);
                return Ok(simulacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("simular")]
        public async Task<IActionResult> SimularEmpresa([FromBody] LiquidationRequest request)
        {
            try
            {
                var companyId = GetCompanyIdFromToken();
                var simulacion = await _liquidationService.SimularLiquidacionAsync(companyId, request);
                return Ok(simulacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("cerrar-mes")]
        public async Task<IActionResult> CerrarMes([FromBody] LiquidationRequest request)
        {
            try
            {
                var companyId = GetCompanyIdFromToken();
                var liquidacion = await _liquidationService.CerrarMesAsync(companyId, request);

                return Ok(new
                {
                    mensaje = "Liquidación cerrada correctamente.",
                    data = new
                    {
                        liquidacionId = liquidacion.Id,
                        periodoLiquidado = liquidacion.LiquidationDate, // Ej: 30/06
                        fechaEjecucion = liquidacion.ExecutionDate,     // Ej: 18/06 20:15
                        total = liquidacion.Total
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("cierre-mes")]
        public async Task<IActionResult> GetCierreMes([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var companyId = GetCompanyIdFromToken();
                var cierre = await _liquidationService.GetCierreMesAsync(companyId, month, year);
                return Ok(cierre);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // NUEVO ENDPOINT: Anula la liquidación
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> AnularCierreMes(int id)
        {
            try
            {
                var companyId = GetCompanyIdFromToken();
                await _liquidationService.DeleteCierreMesAsync(companyId, id);
                return Ok(new { mensaje = "Liquidación anulada correctamente. Ahora puedes volver a simular o cerrar el mes." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}