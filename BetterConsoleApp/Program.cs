using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System;
using System.IO;

//DI, Serilog, Settings

namespace BetterConsoleApp
{
    class ConsoleUI
    {
        static void Main(string[] args)
        {
            //Create a new builder and pass a ref to our config below
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build()) // Logger can read configuration from appsettings.json file
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application starting...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //configure all of services
                    services.AddTransient<IGreetingService, GreetingService>();

                })
                .UseSerilog() // already defined above with the logger configuration
                .Build();

            var svc = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
            svc.Run();
        }

        //log right away before anything else
        static void BuildConfig(IConfigurationBuilder builder)
        {
            //add to builder the ability to talk to appsettings.json.
            //Look for it in the same directory you're running.
            //If it changes, reload it
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true) // get the json for the environment you're running- development, production, etc. If no variable exists, use Production
            .AddEnvironmentVariables();

        }
    }
}
