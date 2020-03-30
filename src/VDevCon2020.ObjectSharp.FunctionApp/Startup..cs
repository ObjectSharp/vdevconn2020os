using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using VDevCon2020.ObjectSharp.FunctionApp.Services;

[assembly: FunctionsStartup(typeof(VDevCon2020.ObjectSharp.FunctionApp.Startup))]

namespace VDevCon2020.ObjectSharp.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ITimestampService, LocalServerTimestampService>();
            builder.Services.AddSingleton<IBackgroundJobWorker, SimpleBackgrounWorker>();
        }
    }
}
