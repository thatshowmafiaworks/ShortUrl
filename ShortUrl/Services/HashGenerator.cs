using System.Security.Cryptography;
using System.Text;

namespace ShortUrl.Services
{
    public class HashGenerator : IHashGenerator
    {
        public string GenerateHash(string originalUrl)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(originalUrl));
                var stringHash = Convert.ToBase64String(hash)
                    .Replace("+", "")
                    .Replace("=", "")
                    .Replace("/", "")
                    .Substring(0, 8);
                return stringHash;
            }
        }
    }
}
