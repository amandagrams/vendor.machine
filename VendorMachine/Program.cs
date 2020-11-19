using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VendorMachine
{
    class Program
    {
        static void Main(string[] args)
        {
           var environmentConfiguration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var environment = environmentConfiguration["ASPNETCORE_ENVIRONMENT"];

           var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IGreetingService, GreetingService>();
                })
                .Build();

            var srv = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
            srv.Run();
        }   
    }
}
