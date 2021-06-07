using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace TestingToken
{
    public class Core : ICore
    {
        public string GenerateToken()
        {
            var rsa = CreateSignature();
            var encoder = GetEncoder(rsa);
            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds();

            // create the payload according to your need 
            // iss is the Service Account Email ID
            var payload = new Dictionary<string, object>
            {
                { "iss",   "sa-sitecore-ct@contenttagging.iam.gserviceaccount.com"},
                { "scope", "https://www.googleapis.com/auth/cloud-platform" },
                { "aud",   "https://oauth2.googleapis.com/token" },
                { "exp",    exp},
                { "iat",    iat},

            };
            //Final token
            var token = encoder.Encode(payload, new byte[0]);
            return token;
        }

        public string GetBearerToken(string token)
        {
            // available in Google API developers' docs
            var grant_type = "urn:ietf:params:oauth:grant-type:jwt-bearer";

            var client = new RestClient("https://oauth2.googleapis.com/token");
            var request = new RestRequest();

            // call API with specific data in headers
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", string.Format(grant_type));
            request.AddParameter("assertion", token);

            var response = client.Post(request);

            // parse bearer token from JSON object returned from API call 
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
            var bearer_token = jsonObj.SelectToken("access_token").ToString();
            return bearer_token;
        }

        public PredictResponse Predict(string bearerToken)
        {
            string googleAIURL = "https://automl.googleapis.com/v1/projects/501813537859/locations/us-central1/models/TCN5733761823828606976:predict";
            var client = new RestClient(googleAIURL);
            var request = new RestRequest(Method.POST);
            
            // call API with specific data and bearer token in headers
            request.AddHeader("postman-token", "b75d66fd-8da1-afc5-2aee-02925185f07e");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + bearerToken);
            request.AddParameter("application/json", "{\r\n  \"payload\": {\r\n    \"textSnippet\": {\r\n      \"content\": \"How is your education?\",\r\n      \"mime_type\": \"text/plain\"\r\n    }\r\n  }\r\n}", ParameterType.RequestBody);
            
            //parse the prediction result returned from API call
            IRestResponse response = client.Execute(request);
            PredictResponse predict = JsonConvert.DeserializeObject<PredictResponse>(response.Content);

            return predict;
        }



        public RSAParameters CreateSignature()
        {
            var rawString =
                "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQD11lmfCcUKi7rC" +
                "bloCYFb18la1uzKhklv4PMH6dNkdb0daBNIQucJ1Ig0qLW228JAYUU/PJN4KdFti" +
                "PM58WwwqAhY/Hxcgd0ArIZOjJI1SM2xRs8Y0TqHxreAPyhn1Hym2dVikg2PwO27e" +
                "cccYknJ1t0vVraO3lpc32ITaFVi9jbyZwt9BEe3A/cZlWk4xLBpAgCcxYBZ1Abyw" +
                "V7wJmbWNcwywsCovtWja9g3wBY7HlDPjkEfuGht6ixrHLsd5fyjtNmtcBKVypHWW" +
                "1hlAtGiSJ/iMdTlIs472T/sU3gjfQNVPGvyI5q7391NI7E0PpM92Ky8WCc7v/2Cb" +
                "QIVkUKATAgMBAAECggEAKEc+wdRGSMG3ZLnTMjEkrlaj/FHigXR1xcihTkQsz9TL" +
                "+k9wjOBSoFcRlHKDAlvh0AoqgFmdrlV9ei1U9i0DY4fvVdeQi3eJDPK1uh94BBrR" +
                "dDAh7loKezWDe4gfvqNPdfSTWcj+ln1hCeiBuAtD3k5HijKmm018vd/8zFp+V/f5" +
                "1OE0yYLTe/C0rVRdnFdSY9BhGht4fv+sYoAsoKoCMpMUhvlZJ1tETrQCTowKw7qb" +
                "73jSHzaGptwL5CJLai13PxdB5SVsWUQILvqrDuZA/IXpjpQmDWpHVwY/ljIDWsJl" +
                "lKyLCmLXvXV9l+F8GAxg1XkWXS2pQB06+yp+5v/FwQKBgQD7dbl72LBB3ybXSWNJ" +
                "SYu21DCthYJhgtOmhg6ovrL9g5US+0N7z+dmTFAWFpSlY4JnJ9XAl0SHipzP550+" +
                "LD6BrbyRTRirb//qSE0873Co5RmRA+/Oe7frrEq6/V5xvoSUps6veQi/D8BgJIZ6" +
                "hg46i80Cx0UHDPrVLEAsFluQswKBgQD6RqMwRl6EO1HGB6KdSh3W/JJeN+mAd9KI" +
                "5rrNo9yhHv/wXJIqIjBS2v8cTiz48N4MWxsKPjj5uvyKJNK0lBBnf7lntKzC8b7/" +
                "T3O6dxPUlxO2EBLunSIt3xYPT1IISsYE9w0GafdNIJoOjQLz/3ji3T26nDBRl3H7" +
                "X/5qg5ujIQKBgCFpbw/ppuHZyMtqUOr3/rx4TU+BIeXbAExsG56IWDgfN7uh70GQ" +
                "SmrpUOrebTnckwMp49kHQG/SFyv41ofgUR3h++BCojiFVTfIC5tBJXMLne4K295P" +
                "ygxihDt8VwQ+EFfAIk5mgqcAbMtZjxTQoytcUA1CdQWOz/VrP8ub3ObJAoGBAJoO" +
                "YyMzHbj9nq9sQb/aQJX2cM6IUjZfC3xzsmckx4lyQI8fHGXNF7vYzBILWQl/kt8+" +
                "MV3TwVSPsiYC1qSLQ7HB4Emi4Hk7P5t+t7j+1XkV6e8248B3fvOHj9eqts8d39hs" +
                "jlr1XWHfgJUWXcv95NaY4wd8Xw3KUEIw67V6/5GhAoGBAMrSJ3JCKfYWlKwM5tMG" +
                "y5zstZX6Jrdzx5g8qHLySD3DcPpanaIaqUmnON4Q2IHPfuWqpXKuPmEWEQ+DNiQI" +
                "9HR3fJGy/JwBBOw037ariGU93ClPRfxRoNmgSVkOchyKHlZd9/AnHDh6HPAHSevI" +
                "0gyNVW0Ihhu4T6wlWNEaIs9+";


            RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters1;
            var keyBytes = Convert.FromBase64String(rawString);
            var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(keyBytes);


            rsaPrivateCrtKeyParameters1 = (RsaPrivateCrtKeyParameters)asymmetricKeyParameter;

            RSAParameters r = DotNetUtilities.ToRSAParameters(rsaPrivateCrtKeyParameters1);

            return r;
        }

        public IJwtEncoder GetEncoder(RSAParameters rSAParameters)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(rSAParameters);
            var algorithm = new RS256Algorithm(csp, csp);
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder;
        }
    }
}
