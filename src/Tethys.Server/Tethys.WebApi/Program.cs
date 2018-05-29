using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Tethys.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (!ExtractArgs(args, out var tethysConfig))
                return -1;
            CreateWebHostBuilder(args, tethysConfig).Build().Run();
            return 0;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, TethysConfig config)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:" + config.HttpPort);
        }

        private static bool ExtractArgs(IEnumerable<string> args, out TethysConfig tethysConfig)
        {
            var errors = new List<string>();
            tethysConfig = CommandLineArgsParser.Parse(args, errors);
            if (!errors.Any()) return true;

            Console.WriteLine("FAILED!!!\n\t" + string.Join("\n\t", errors.ToArray()));
            return false;
        }
    }
}
