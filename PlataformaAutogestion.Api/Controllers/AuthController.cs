using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Infrastructure.Services;
using System.Security.Claims;

namespace PlataformaAutogestion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string MfaScheme = "MfaBearer";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(
                request.UserName,
                request.Password);

            return Ok(new { token = result.Token, mfaRequired = result.MfaRequired, mfaSetupRequired = result.MfaSetupRequired });
        }

        // Solo con el token temporal de configuración: genera el secreto y devuelve la URI para el QR.
        [HttpPost("mfa/setup")]
        [Authorize(AuthenticationSchemes = MfaScheme)]
        public async Task<IActionResult> MfaSetup()
        {
            if (GetMfaPurpose() != AuthService.MfaPurposeSetup)
                return Unauthorized();

            var result = await _authService.MfaSetupAsync(GetMfaUserId());
            return Ok(new { otpAuthUri = result.OtpAuthUri, secret = result.Secret });
        }

        // Valida el código de 6 dígitos y recién ahí entrega el JWT real.
        [HttpPost("mfa/verify")]
        [Authorize(AuthenticationSchemes = MfaScheme)]
        public async Task<IActionResult> MfaVerify(MfaVerifyRequest request)
        {
            var purpose = GetMfaPurpose();
            if (purpose != AuthService.MfaPurposeSetup && purpose != AuthService.MfaPurposeVerify)
                return Unauthorized();

            var token = await _authService.MfaVerifyAsync(
                GetMfaUserId(),
                request.Code,
                purpose == AuthService.MfaPurposeSetup);

            return Ok(new { token });
        }

        private string? GetMfaPurpose() => User.FindFirstValue(AuthService.MfaPurposeClaim);

        private int GetMfaUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request.Email);
            return Ok(new { message = "Te enviamos un correo para restablecer tu contraseña." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            return Ok(new { message = "Tu contraseña fue actualizada correctamente." });
        }
    }
}