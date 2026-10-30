using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    // Resultado del login. Si MfaRequired o MfaSetupRequired es true, Token es un token
    // temporal que solo sirve para los endpoints de MFA, no para la API de negocio.
    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
        public bool MfaRequired { get; set; }
        public bool MfaSetupRequired { get; set; }
    }

    public class MfaSetupResult
    {
        public string OtpAuthUri { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }

    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string userName, string password);
        Task<MfaSetupResult> MfaSetupAsync(int userId);
        Task<string> MfaVerifyAsync(int userId, string code, bool isSetup);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(string email, string token, string newPassword);
    }
}
