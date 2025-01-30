using System;
using System.Security.Cryptography;
using System.Text;

namespace CarRentalApp.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string inputPassword)
        {
            return HashPassword(inputPassword) == hashedPassword;
        }
    }
}