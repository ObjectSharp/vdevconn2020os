using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using VDevCon2020.ObjectSharp.FunctionApp.Services;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class ServeFunctions
    {
        [FunctionName(nameof(HttpServeHtml))]
        public async Task<HttpResponseMessage> HttpServeHtml(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "serve/{filename}")] HttpRequest req,
            string filename,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var host = $"{req.Scheme}://{req.Host}";

            var path = GetPath(filename);
            log.LogInformation($"Serving {path}");
            using var stream = new FileStream(path, FileMode.Open);

            // This causes buffering so that we can do a replace
            // When a replace is not required, it is better to use
            // response.Content = new StreamContent(stream)
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            content = content.Replace("__API_URL__", host);
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private string GetPath(string filename)
        {
            var root = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrWhiteSpace(root))
                root = Path.Combine(root, @"site\wwwroot\wwwroot");
            else
                root = Path.Combine(Environment.CurrentDirectory, @"wwwroot");

            return Path.Combine(root, filename);
        }
    }
}
