using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using System.Security.Claims;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailLiquidationController : ControllerBase
    {
        private readonly IDetailLiquidationService _detailLiquidationService;
        public DetailLiquidationController(IDetailLiquidationService detailLiquidationService)
        {
            _detailLiquidationService = detailLiquidationService;
        }

        [Authorize]
        [HttpGet("mis-liquidaciones")]
        public async Task<IActionResult> GetByUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var details = await _detailLiquidationService.GetByUserAsync(userId);
            return Ok(details);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("porLiquidacion/{idLiquidation:int}")]
        public async Task<IActionResult> GetByLiquidation(int idLiquidation)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var details = await _detailLiquidationService.GetByLiquidationAsync(idLiquidation, companyId);
            return Ok(details);
        }
    }
}
