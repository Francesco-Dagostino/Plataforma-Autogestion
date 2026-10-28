using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Domain.Security;
using PlataformaAutogestion.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthServiceOptions _options;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            ApplicationDbContext context,
            IOptions<AuthServiceOptions> options,
            IEmailService emailService,
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IConfiguration configuration)
        {
            _context = context;
            _options = options.Value;
            _emailService = emailService;
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null || !user.IsActive || !PasswordHasher.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            if (!PasswordHasher.IsHashed(user.Password))
            {
                user.Password = PasswordHasher.Hash(password);
                await _context.SaveChangesAsync();
            }

            return GenerateToken(user);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            // Valida que el email pertenezca a un usuario registrado
            if (user == null || !user.IsActive)
                throw new InvalidOperationException("El correo ingresado no se encuentra registrado.");

            // Invalidar tokens anteriores
            await _resetRepository.InvalidateAllForUserAsync(user.Id);

            // Generar token seguro de 64 bytes
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var tokenPlain = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            // Guardar hash SHA256 del token
            var tokenHash = HashToken(tokenPlain);

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                CreatedAt = DateTime.UtcNow,
                Used = false
            };

            await _resetRepository.AddAsync(resetToken);

            // Construir link de reset
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
            var resetLink = $"{frontendUrl}/reset-password?token={tokenPlain}&email={Uri.EscapeDataString(email)}";
            Console.WriteLine($">>> [LINK DE RECUPERACIÓN]: {resetLink}");

            // Enviar email
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;'>
                    <h2 style='color: #102848;'>Recuperar contraseña</h2>
                    <p>Hola <strong>{user.Name}</strong>,</p>
                    <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                    <p>Hacé clic en el siguiente botón:</p>
                    <a href='{resetLink}' 
                       style='display: inline-block; background-color: #059669; color: white; 
                              padding: 12px 24px; text-decoration: none; border-radius: 8px; 
                              font-weight: bold;'>
                        Restablecer contraseña
                    </a>
                    <p style='margin-top: 20px; color: #666; font-size: 13px;'>
                        Este enlace expira en 30 minutos. Si no solicitaste este cambio, ignorá este email.
                    </p>
                </div>";

            await _emailService.SendEmailAsync(user.Email, "Recuperar contraseña - Autogestión", htmlBody);
        }

        public async Task ResetPasswordAsync(string email, string token, string newPassword)
        {
            var tokenHash = HashToken(token);
            var resetToken = await _resetRepository.GetByTokenHashAsync(tokenHash);

            if (resetToken == null)
                throw new InvalidOperationException("El enlace de recuperación no es válido.");

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("El enlace de recuperación ha expirado.");

            if (resetToken.User == null || !resetToken.User.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("El enlace de recuperación no es válido.");

            // Actualizar contraseña
            resetToken.User.Password = PasswordHasher.Hash(newPassword);
            await _userRepository.UpdateAsync(resetToken.User);

            // Marcar token como usado
            resetToken.Used = true;
            await _resetRepository.UpdateAsync(resetToken);
        }

        // Hashea el token con SHA256 para almacenamiento seguro
        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_options.SecretForKey));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.role.ToString()),
            };

            if (user.IdCompany.HasValue)
                claims.Add(new Claim("IdCompany", user.IdCompany.Value.ToString()));

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class AuthServiceOptions
    {
        public const string AutenticacionService = "AutenticacionService";

        public string SecretForKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}