using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QnAMakerMessagesDemo
{
    public class TokenGenerator
    {
        public static string GenerateToken(IConfiguration config)
        {
            // retrieve appID and privateKey from configuration
            var appId = config["Authentication:appId"];
            var priavteKeyPath = config["Authentication:privateKey"];
            string privateKey = "";
            using (var reader = File.OpenText(priavteKeyPath)) // file containing RSA PKCS1 private key
                privateKey = reader.ReadToEnd();

            //generate claims list
            const int SECONDS_EXPIRY = 3600;
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var iat = new Claim("iat", ((Int32)t.TotalSeconds).ToString(), ClaimValueTypes.Integer32); // Unix Timestamp for right now
            var application_id = new Claim("application_id", appId); // Current app ID
            var exp = new Claim("exp", ((Int32)(t.TotalSeconds + SECONDS_EXPIRY)).ToString(), ClaimValueTypes.Integer32); // Unix timestamp for when the token expires
            var jti = new Claim("jti", Guid.NewGuid().ToString()); // Unique Token ID
            var claims = new List<Claim>() { iat, application_id, exp, jti };

            //create rsa parameters
            RSAParameters rsaParams;
            using (var tr = new StringReader(privateKey))
            {
                var pemReader = new PemReader(tr);
                var kp = pemReader.ReadObject();
                var privateRsaParams = kp as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            //generate and return JWT
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                Dictionary<string, object> payload = claims.ToDictionary(k => k.Type, v => (object)v.Value);
                return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
            }
        }
    }
}
