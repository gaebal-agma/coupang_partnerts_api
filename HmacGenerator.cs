using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace coupang_partnerts_api
{
   public static class HmacGenerator
    {
        public static String GenerateHmac(String method, String url, String accessKey, String secretKey)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString("yyMMddTHHmmssZ");
            var message = String.Format("{0}{1}{2}", dateTimeStamp, method, url.Replace("?", String.Empty));

            var keyBytes = ASCIIEncoding.Default.GetBytes(secretKey);
            var messageBytes = ASCIIEncoding.Default.GetBytes(message);

            var hashBytes = default(Byte[]);
            using (var hash = new HMACSHA256(keyBytes))
            {
                hashBytes = hash.ComputeHash(messageBytes);
            }

            var signature = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();
            return String.Format("CEA algorithm=HmacSHA256, access-key={0}, signed-date={1}, signature={2}", accessKey, dateTimeStamp, signature);
        }

    }
}
