using System;
using System.Threading.Tasks;
using VDevCon2020.ObjectSharp.FunctionApp.Models;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class SimpleBackgrounWorker : IBackgroundJobWorker
    {
        public async Task Process(BackgroundJob job)
        {
            var random = new Random();
            var sleep = random.Next(job.MinTaskLength, job.MaxTaskLength);
            await Task.Delay(sleep);
        }

        public Task<TOut> Process<TIn, TOut>(BackgroundJob<TIn> job)
        {
            throw new NotImplementedException();
        }
    }
}