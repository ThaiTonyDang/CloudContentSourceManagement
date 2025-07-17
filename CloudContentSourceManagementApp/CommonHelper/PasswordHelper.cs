using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudContentSourceManagementApp.CommonHelper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public static bool VerifyPassword(string plainPassword, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
        }

        public static string Encrypt(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encryptedText)
        {
            var bytes = Convert.FromBase64String(encryptedText);
            var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
