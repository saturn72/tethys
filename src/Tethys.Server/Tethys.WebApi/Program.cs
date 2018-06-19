using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Tethys.WebApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static int Main(string[] args)
        {
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = TethysConfig.FromConfiguration(Configuration);
            var urls = config.HttpPorts.Select(hp => "http://localhost:" + hp).ToArray();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
                .UseStartup<Startup>()
                .UseUrls(urls);
        }
    }
}
