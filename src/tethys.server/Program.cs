﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Tethys.Server
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                var configFilePath = args.Length >= 2 ? args[1] : "appsettings.json";
                var configDirectory = Path.GetDirectoryName(configFilePath);
                if (configDirectory.Length == 0)
                    configDirectory = Directory.GetCurrentDirectory();

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(configDirectory)
                    .AddJsonFile(configFilePath)
                    .AddCommandLine(args)
                    .Build();

                CreateWebHostBuilder(args).Build().Run();
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


        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = TethysConfig.FromConfiguration(Configuration);
            var urls = config.HttpPorts.Select(hp => "http://localhost:" + hp).ToArray();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
                .UseStartup<Startup>()
                .UseSerilog()
                .UseUrls(urls);
        }
    }
}
