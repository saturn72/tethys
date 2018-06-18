using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Tethys.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (!GetTethysConfig(args, out var tethysConfig))
                return -1;

            CreateWebHostBuilder(args, tethysConfig).Build().Run();
            return 0;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, TethysConfig config)
        {
            var urls = config.HttpPorts.Select(hp => "http://localhost:" + hp).ToArray();

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(urls);
        }

        private static bool GetTethysConfig(IEnumerable<string> args, out TethysConfig tethysConfig)
        {
            var errors = new List<string>();
            tethysConfig = CommandLineArgsParser.Load(args, errors);
            if (!errors.Any()) return true;

            Console.WriteLine("FAILED!!!\n\t" + string.Join("\n\t", errors.ToArray()));
            return false;
        }
    }
}
