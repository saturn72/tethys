using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tethys.Server
{
    public class Program
    {
        private static IConfiguration _configuration;

        public static int Main(string[] args)
        {
            var configFilePath = args.Length >= 2 ? args[1] : "appsettings.json";
            var configDirectory = Path.GetDirectoryName(configFilePath);
            if (configDirectory.Length == 0)
                configDirectory = Directory.GetCurrentDirectory();

            _configuration = new ConfigurationBuilder()
                .SetBasePath(configDirectory)
                .AddJsonFile(configFilePath)
                .AddCommandLine(args)
                .Build();

            CreateWebHostBuilder(args).Build().Run();
            return 0;
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = TethysConfig.FromConfiguration(_configuration);
            var urls = config.HttpPorts.Select(hp => "http://localhost:" + hp).ToArray();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(_configuration)
                .UseStartup<Startup>()
                .UseUrls(urls);
        }
    }
}
