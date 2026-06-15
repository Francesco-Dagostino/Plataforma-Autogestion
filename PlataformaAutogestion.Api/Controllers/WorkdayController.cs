using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Exceptions;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
        private readonly IWorkdayService _workdayService;
        public WorkdayController(IWorkdayService workdayService)
        {
            _workdayService = workdayService;
        }

        // Operario: ver sus propias jornadas
        [Authorize]
        [HttpGet("mis-horas/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var workdays = await _workdayService.GetByUserAsync(userId);
            return Ok(workdays);
        }

        // Admin: ver jornadas pendientes de su empresa
        [Authorize(Roles = "Admin")]
        [HttpGet("pendientes/{companyId}")]
        public async Task<IActionResult> GetPending(int companyId)
        {
            var workdays = await _workdayService.GetPendingByCompanyAsync(companyId);
            return Ok(workdays);
        }

        // Operario: cargar jornada
        [Authorize]
        [HttpPost("cargar")]
        public async Task<IActionResult> Add(WorkdayCreateRequest request)
        {
            try
            {
                var workday = await _workdayService.AddAsync(request);
                return Ok(workday);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Admin: aprobar jornada
        [Authorize(Roles = "Admin")]
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            try
            {
                await _workdayService.ApproveAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Admin: rechazar jornada
        [Authorize(Roles = "Admin")]
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            try
            {
                await _workdayService.RejectAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
