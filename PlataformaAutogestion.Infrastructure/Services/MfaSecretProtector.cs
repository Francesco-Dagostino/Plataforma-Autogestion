using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Services
{
    // Cifra el secreto TOTP en reposo con AES-256-GCM. La clave (32 bytes en Base64) viene de la
    // configuración "Mfa:EncryptionKey", así un volcado de la base no alcanza para generar códigos.
    public class MfaSecretProtector
    {
        private const string Prefix = "v1:";
        private const int NonceSize = 12;
        private const int TagSize = 16;

        private readonly byte[] _key;

        public MfaSecretProtector(IConfiguration configuration)
        {
            var base64 = configuration["Mfa:EncryptionKey"];

            if (string.IsNullOrWhiteSpace(base64))
                throw new InvalidOperationException("Falta configurar Mfa:EncryptionKey (32 bytes en Base64).");

            try { _key = Convert.FromBase64String(base64); }
            catch (FormatException) { throw new InvalidOperationException("Mfa:EncryptionKey no es Base64 válido."); }

            if (_key.Length != 32)
                throw new InvalidOperationException("Mfa:EncryptionKey debe ser de 32 bytes (AES-256).");
        }

        public string Protect(string plainText)
        {
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var plain = Encoding.UTF8.GetBytes(plainText);
            var cipher = new byte[plain.Length];
            var tag = new byte[TagSize];

            using var aes = new AesGcm(_key, TagSize);
            aes.Encrypt(nonce, plain, cipher, tag);

            return Prefix + Convert.ToBase64String(nonce.Concat(tag).Concat(cipher).ToArray());
        }

        public string Unprotect(string stored)
        {
            // Secretos guardados antes de activar el cifrado (texto Base32): se aceptan tal cual.
            if (!stored.StartsWith(Prefix))
                return stored;

            var data = Convert.FromBase64String(stored[Prefix.Length..]);
            if (data.Length < NonceSize + TagSize)
                throw new CryptographicException("Secreto MFA corrupto.");

            var nonce = data[..NonceSize];
            var tag = data[NonceSize..(NonceSize + TagSize)];
            var cipher = data[(NonceSize + TagSize)..];
            var plain = new byte[cipher.Length];

            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(nonce, cipher, tag, plain);

            return Encoding.UTF8.GetString(plain);
        }
    }
}
