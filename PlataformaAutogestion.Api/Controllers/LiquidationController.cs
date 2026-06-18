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

        [HttpGet("empleado/{userId}/simular")]
        public async Task<IActionResult> SimularSueldoEmpleado(
            int userId,
            [FromQuery] int month,
            [FromQuery] int year)
        {
            try
            {
                var simulacion = await _liquidationService
                    .SimularSueldoEmpleadoAsync(userId, month, year);

                return Ok(simulacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("simular")]
        public async Task<IActionResult> SimularEmpresa(
            [FromBody] LiquidationRequest request)
        {
            try
            {
                var simulacion = await _liquidationService
                    .SimularLiquidacionAsync(request);

                return Ok(simulacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("cerrar-mes")]
        public async Task<IActionResult> CerrarMes(
            [FromBody] LiquidationRequest request)
        {
            try
            {
                var liquidacion = await _liquidationService
                    .CerrarMesAsync(request);

                return Ok(new
                {
                    mensaje = "Liquidación cerrada correctamente.",
                    data = liquidacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}