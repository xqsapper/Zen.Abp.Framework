using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Zen.Abp.Framework.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = GetLoggerConfig();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            try
            {
                Log.Information("Starting web host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var hostUrls = GetHostUrls();
                    if (hostUrls != null && hostUrls.Any())
                    {
                        webBuilder = webBuilder.UseUrls(hostUrls);
                    }

                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
        }

        private static string[] GetHostUrls()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            var hostUrls = builder.GetSection("HostUrls").Value;
            if (string.IsNullOrWhiteSpace(hostUrls))
            {
                return new string[0];
            }

            var urls = hostUrls.Replace(";", ",")
                .Split(",").Distinct()
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();
            return urls;
        }


        private static IConfigurationRoot GetLoggerConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("logsettings.json", false, true)
                .Build();
            return configuration;
        }
    }
}