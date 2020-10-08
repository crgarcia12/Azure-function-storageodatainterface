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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // Validar usuario+Password
            if (!CredentialsValidator.ValidateUserPassword(req))
            {
                return new ForbidResult();
            }

            // Extract OData query string from request
            string queryString = req.QueryString.ToUriComponent();

            // Query Azure Storage
            string response = await AzureStorage.QueryOData(queryString);

            return new OkObjectResult(response);
        }
    }
}
