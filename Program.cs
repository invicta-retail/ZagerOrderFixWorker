using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Repositorio.Interfaces;
using ZagerOrderFixWorker.Repositorio.Queries;
using ZagerOrderFixWorker.Services.Interfaces;
using ZagerOrderFixWorker.Services.ZagerAPI;

namespace ZagerOrderFixWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
         .WriteTo.File("ZagerOrderFixWorker.log", rollingInterval: RollingInterval.Day)
         .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<MainProcess>().StartService(args);

            var logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation("Completed process");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog())
                    .AddTransient<MainProcess>()
                    .AddTransient<IZagerService,ZagerService>()
                    .AddTransient<IShippersConfirmationQueries,ShippersConfirmationQueries>();
        }
    }
}
