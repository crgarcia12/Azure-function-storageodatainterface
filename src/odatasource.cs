using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net;
using System.Text;

namespace funcodata
{
    public static class odatasource
    {
        [FunctionName("odatasource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var storageAccountName = "crgarodatastorage";
            var storageAccountkey = "<key>";
            var tableName = "demotable";

            var apiVersion = "2017-04-17";
            var tableURL = $"https://{storageAccountName}.table.core.windows.net/{tableName}";
            var GMTime = (DateTime.UtcNow).ToString("R");
            var sig = GMTime + "\n" + $"/{storageAccountName}/{tableName}";
            byte[] secretkey = new Byte[64];

            string signature = "";
            byte[] key = Base64Decode(storageAccountkey);
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                signature = Convert.ToBase64String(hmac.ComputeHash(UTFGetBytes(sig)));
            } 

            
            string html = string.Empty;
            //?$top=5
            string url = $"{tableURL}{req.QueryString.ToUriComponent()}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Headers["Authorization"]  = "SharedKeyLite " + storageAccountName + ":" + signature;
            request.Headers["Accept"]         = "application/json;odata=fullmetadata";
            request.Headers["x-ms-date"]      = GMTime;
            request.Headers["x-ms-version"]   = apiVersion;
            request.Headers["MaxDataServiceVersion"] = "3.0;NetFx";
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            log.LogInformation(html);

            return new OkObjectResult(html);
        }

        public static byte[] Base64Decode(string base64EncodedData) {
            return System.Convert.FromBase64String(base64EncodedData);
        }

        public static byte[] UTFGetBytes(string message)
        {
            Encoding u8 = Encoding.UTF8;
            return u8.GetBytes(message);
        }
    }


}
