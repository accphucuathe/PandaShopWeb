using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BMT_DATN.Models
{
    public class EncodingHelper
    {
        public string GenerateSalt()
        {
            var bytes = new byte[128 / 2];          // day 64 bytes
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string ComputeHash(string stringToHash, string saltString)
        {
            byte[] bytesToHash = Encoding.UTF8.GetBytes(stringToHash);
            byte[] salt = Encoding.UTF8.GetBytes(saltString);
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(64));
        }
    }
}