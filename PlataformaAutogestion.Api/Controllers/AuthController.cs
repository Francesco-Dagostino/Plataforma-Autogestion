using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Interfaces;

namespace PlataformaAutogestion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(
                request.UserName,
                request.Password);

            return Ok(new { token });
        }
    }
}