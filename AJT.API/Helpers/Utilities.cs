using System;
using System.Security.Cryptography;
using System.Text;

namespace AJT.API.Helpers
{
    public class Utilities
    {
        public static string GetHash(string baseString, string key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            var encoding = new ASCIIEncoding();

            var textBytes = encoding.GetBytes(baseString);
            var keyBytes = encoding.GetBytes(key);

            byte[] hashBytes;

            using (var hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
