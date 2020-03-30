using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using VDevCon2020.ObjectSharp.FunctionApp.Models;
using VDevCon2020.ObjectSharp.FunctionApp.Services;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class BackgroundJobFunctions
    {
        private readonly ITimestampService _timestamps;
        private readonly IBackgroundJobWorker _worker;

        public BackgroundJobFunctions(ITimestampService timestamps, IBackgroundJobWorker worker)
        {
            _timestamps = timestamps;
            _worker = worker;
        }

        [FunctionName(nameof(HttpCreateJob))]
        public async Task<IActionResult> HttpCreateJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jobs")] HttpRequestMessage req,
            [DurableClient]IDurableClient durableClient,
            ILogger log)
        {
            var job = await req.Content.ReadAsAsync<BackgroundJob>();
            job.CreatedAt = _timestamps.Now();
            job.Id = await durableClient.StartNewAsync(nameof(BackgroundJobOrchestrator), job);

            return new OkObjectResult(job);
        }

        [FunctionName(nameof(HttpGetJobs))]
        public async Task<IActionResult> HttpGetJobs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs")] HttpRequestMessage req,
            [DurableClient]IDurableClient durableClient,
            ILogger log)
        {
            var durableTasks = await durableClient.GetStatusAsync();

            return new OkObjectResult(durableTasks);
        }

        [FunctionName(nameof(HttpGetJob))]
        public async Task<IActionResult> HttpGetJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs/{id}")] HttpRequestMessage req,
            [DurableClient]IDurableClient durableClient,
            string id,
            ILogger log)
        {
            var durableTasks = await durableClient.GetStatusAsync(id);

            return new OkObjectResult(durableTasks);
        }

        [FunctionName(nameof(HttpDeleteJob))]
        public async Task<IActionResult> HttpDeleteJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "jobs/{id}")] HttpRequestMessage req,
            [DurableClient]IDurableClient durableClient,
            string id,
            ILogger log)
        {
            var result = await durableClient.PurgeInstanceHistoryAsync(id);

            return new OkObjectResult(result);
        }

        [FunctionName(nameof(BackgroundJobOrchestrator))]
        public async Task<BackgroundJobStatus> BackgroundJobOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            // Get input and setup background job
            var job = context.GetInput<BackgroundJob>();
            
            // Create a custom status object to track progress
            var status = new BackgroundJobStatus { 
                BatchSize = job.BatchSize,
                CreatedAt = job.CreatedAt,
                Id = context.InstanceId,
                MaxTaskLength = job.MaxTaskLength,
                MinTaskLength = job.MinTaskLength,
                Name = job.Name, 
                TaskCount = job.TaskCount
            };
            if (!context.IsReplaying)
            {
                log.LogWarning($"[{job.Id}] Starting new background job with {job.TaskCount} tasks");
                status.Message = "Initializing...";
            }

            // The orchestrator cannot do any task related work, in fact any attempt to await in here, other then
            // launching an activity will throw an exception
            while (status.Progress < 100)
            {
                status = await context.CallActivityAsync<BackgroundJobStatus>(nameof(ActivityBackgroundJobWorker), status);
                context.SetCustomStatus(status);
            }
            
            // Clear out the status in the orchestrator because the work is done and will be returned in the output
            context.SetCustomStatus(null);

            return status;
        }

        [FunctionName(nameof(ActivityBackgroundJobWorker))]
        public async Task<BackgroundJobStatus> ActivityBackgroundJobWorker(
            [ActivityTrigger] BackgroundJobStatus job,
            [SignalR(HubName = SignalRFunctions.HubName)] IAsyncCollector<SignalRMessage> signalr,
            ILogger log)
        {
            var sw = Stopwatch.StartNew();
            var progress = job.Progress;
            while (progress >= job.Progress) 
            {
                // Use the inject worker (processor) to perform the work
                await _worker.Process(job);

                // Update stats
                job.CompletedTaskCount += 1;
                job.ElapsedMiliseconds += sw.ElapsedMilliseconds;
                job.Message = job.Progress < 100 ? "Processing" : "Complete";
            }
            log.LogWarning($"[{job.Id}] Completed {job.CompletedTaskCount} of {job.TaskCount} [{job.Progress}%]");

            // Broadcast progress updates to subscribes
            await signalr.AddAsync(SignalRFunctions.CreateMessage(job));
            sw.Stop();

            // Return updated status
            return job;
        }
    }
}