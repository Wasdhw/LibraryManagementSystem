using System;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagementSystem.Utils
{
    public static class Security
    {
        public static string HashPassword(string password)
        {
            if (password == null) return string.Empty;
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hashed)
        {
            return string.Equals(HashPassword(password), hashed, StringComparison.OrdinalIgnoreCase);
        }

        public static string GenerateTemporaryPassword(int length = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789@#$%";
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[data[i] % chars.Length]);
            }
            return sb.ToString();
        }
    }
}

