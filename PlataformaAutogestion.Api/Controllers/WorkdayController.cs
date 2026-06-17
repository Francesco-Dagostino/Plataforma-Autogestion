using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Request;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet("mis-horas")]
        public async Task<IActionResult> GetByUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var workdays = await _workdayService.GetByUserAsync(userId);
            return Ok(workdays);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("PendientesDeAprobacion")]
        public async Task<IActionResult> GetPending()
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var workdays = await _workdayService.GetPendingByCompanyAsync(companyId);
            return Ok(workdays);
        }

        [Authorize]
        [HttpPost("cargar")]
        public async Task<IActionResult> Add(WorkdayCreateRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var workday = await _workdayService.AddAsync(request, userId);
            return Ok(workday);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            await _workdayService.ApproveAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            await _workdayService.RejectAsync(id);
            return NoContent();
        }
    }
}