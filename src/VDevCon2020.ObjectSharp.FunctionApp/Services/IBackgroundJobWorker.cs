using System.Threading.Tasks;
using VDevCon2020.ObjectSharp.FunctionApp.Models;

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public interface IBackgroundJobWorker
    {
        Task Process(BackgroundJob job);
        Task<TOut> Process<TIn, TOut>(BackgroundJob<TIn> job);
    }
}