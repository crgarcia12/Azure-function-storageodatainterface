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
    public static class CredentialsValidator
    {       

        static string user = "carlos";
        static string pass = "carlospassword";

        public static bool ValidateUserPassword(HttpRequest req)
        {
            string base64Token = ((string)req.Headers["Authorization"]).Split(' ')[1];
            string[] usernameAndPassword = Base64Decode(base64Token).Split(':');

            return (user == usernameAndPassword[0] && pass == usernameAndPassword[1]);
        }
        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}