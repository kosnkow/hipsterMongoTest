using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using JHipsterNet.Logging;
using ILogger = Serilog.ILogger;

using static JHipsterNet.Boot.BannerPrinter;

namespace MyCompany {
    public class Program {

        public static int Main(string[] args)
        {
            PrintBanner(10 * 1000);

            try {

                Log.Logger = CreateLogger();

                CreateWebHostBuilder(args).Build().Run();

                return 0;

            }
            catch (Exception ex) {
                // Use ForContext to give a context to this static environment (for Serilog LoggerNameEnricher).
                Log.ForContext<Program>().Fatal(ex, $"Host terminated unexpectedly");
                return 1;

            }
            finally {

                Log.CloseAndFlush();

            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(params string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }

        /// <summary>
        /// Create application logger from configuration.
        /// </summary>
        /// <returns></returns>
        private static ILogger CreateLogger()
        {
            var appConfiguration = GetAppConfiguration();

            // for logger configuration
            // https://github.com/serilog/serilog-settings-configuration
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<LoggerNameEnricher>()
                .ReadFrom.Configuration(appConfiguration);

            return loggerConfiguration.CreateLogger();
        }

        /// <summary>
        /// Gets the current application configuration
        /// from global and specific appsettings.
        /// </summary>
        /// <returns>Return the application <see cref="IConfiguration"/></returns>
        private static IConfiguration GetAppConfiguration()
        {
            // Actually, before ASP.NET bootstrap, we must rely on environment variable to get environment name
            // https://docs.microsoft.com/fr-fr/aspnet/core/fundamentals/environments?view=aspnetcore-2.2
            // Pay attention to casing for Linux environment. By default it's pascal case.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
