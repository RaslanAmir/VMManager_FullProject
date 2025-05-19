using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace VMManager.Common
{
    public static class SecureStringHelper
    {
        public static SecureString? ToSecureString(string? plainString)
        {
            if (string.IsNullOrEmpty(plainString))
                return null;

            SecureString secure = new SecureString();
            foreach (char c in plainString)
                secure.AppendChar(c);

            secure.MakeReadOnly();
            return secure;
        }

        public static string ToPlainString(SecureString? secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString) ?? string.Empty;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string Encrypt(string? plainText)
        {
            if (plainText == null)
                return string.Empty;

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string? encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            try
            {
                byte[] data = Convert.FromBase64String(encryptedText);
                byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
