using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using VDevCon2020.ObjectSharp.FunctionApp.Models;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class SignalRFunctions
    {
        public const string HubName = "vdevconn2020";
        public const string HubStatusUpdateTarget = "status-update";

        [FunctionName(nameof(HttpConnectToHub))]
        public SignalRConnectionInfo HttpConnectToHub(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "signalr/connect")] HttpRequest req,
            [SignalRConnectionInfo(HubName = HubName)] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName(nameof(HttpSendToHub))]
        public async Task<IActionResult> HttpSendToHub(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "signalr/send")] HttpRequestMessage req,
            [SignalR(HubName = HubName)] IAsyncCollector<SignalRMessage> signalr)
        {
            var status = await req.Content.ReadAsAsync<BackgroundJobStatus>();

            await signalr.AddAsync(CreateMessage(status));
            return new OkResult();
        }

        public static SignalRMessage CreateMessage(BackgroundJobStatus payload)
        {
            var message = new SignalRMessage
            {
                Target = HubStatusUpdateTarget,
                Arguments = new[] { payload }
            };
            return message;
        }
    }
}
