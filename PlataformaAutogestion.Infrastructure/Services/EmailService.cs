using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PlataformaAutogestion.Domain.Interfaces;

namespace PlataformaAutogestion.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailServiceOptions _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailServiceOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            // Si no se configuraron credenciales reales, se simula el envío para pruebas locales
            if (string.IsNullOrWhiteSpace(_options.SenderPassword) || 
                _options.SenderPassword.StartsWith("TU_") || 
                _options.SenderEmail.StartsWith("TU_"))
            {
                _logger.LogWarning("EmailService: Credenciales SMTP no configuradas. Simulación de envío a {To} con asunto '{Subject}'.", to, subject);
                return;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_options.SenderEmail, _options.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo SMTP a {To}", to);
                throw new InvalidOperationException("No se pudo enviar el correo de recuperación. Verifique las credenciales SMTP.", ex);
            }
        }
    }
}
