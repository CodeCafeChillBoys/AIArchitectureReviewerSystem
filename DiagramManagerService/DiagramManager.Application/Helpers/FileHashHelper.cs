using System.Security.Cryptography;
using System.Text;

namespace DiagramManager.Application.Helpers
{
    public static class FileHashHelper
    {
        public static string ComputeSha256(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(fileBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
