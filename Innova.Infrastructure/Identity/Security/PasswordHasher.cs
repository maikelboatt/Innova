// Innova.Infrastructure/Identity/Security/PasswordHasher.cs

using System.Security.Cryptography;
using Innova.Application.Abstractions.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Innova.Infrastructure.Identity.Security
{
    /// <summary>
    ///     PBKDF2 (HMAC-SHA256) password hashing via
    ///     Microsoft.AspNetCore.Cryptography.KeyDerivation's low-level
    ///     KeyDerivation.Pbkdf2 primitive.
    ///     Stored format: {iterations}.{base64(salt)}.{base64(hash)}
    ///     — self-describing, so the iteration count can be increased
    ///     later without invalidating already-stored hashes (Verify
    ///     reads whatever count was used at hash time).
    /// </summary>
    public sealed class PasswordHasher:IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public string Hash( string password )
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                Iterations,
                HashSize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify( string passwordHash, string providedPassword )
        {
            string[] parts = passwordHash.Split('.', 3);
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int iterations))
                return false;

            byte[] salt, expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expectedHash = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actualHash = KeyDerivation.Pbkdf2(
                providedPassword,
                salt,
                KeyDerivationPrf.HMACSHA256,
                iterations,
                expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
