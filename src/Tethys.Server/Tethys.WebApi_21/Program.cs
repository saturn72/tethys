using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Tethys.WebApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static int Main(string[] args)
        {
            if (!ExtractArgs(args, out var tethysConfig))
                return -1;
            ConfigureLog();

            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args, tethysConfig).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureLog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Tethys", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static bool ExtractArgs(string[] args, out TethysConfig tethysConfig)
        {
            var errors = new List<string>();
            tethysConfig = CommandLineArgsParser.Parse(args, errors);
            if (!errors.Any()) return true;

            Console.WriteLine("FAILED!!!\n\t" + string.Join("\n\t", errors.ToArray()));
            return false;
        }

        public static IWebHost BuildWebHost(string[] args, TethysConfig config) =>

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:" + config.HttpPort)
                .UseSerilog()
                .Build();
    }
}
