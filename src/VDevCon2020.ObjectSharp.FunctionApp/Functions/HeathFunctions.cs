using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using VDevCon2020.ObjectSharp.FunctionApp.Services;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class HeathFunctions
    {
        private readonly ITimestampService _timestamps;

        public HeathFunctions(ITimestampService timestamps)
        {
            _timestamps = timestamps;
        }

        [FunctionName(nameof(HttpHealthCheck))]
        public IActionResult HttpHealthCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req,
            ILogger log)
        {
            log.LogWarning("Health check hit. This should be logged.");

            var timestamp = _timestamps.Now();
            return new OkObjectResult(new { timestamp });
        }
    }
}
