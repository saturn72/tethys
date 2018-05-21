using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Tethys.WebApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var errors = new List<string>();
            var tethysConfig = CommandLineArgsParser.Parse(args, errors);
            if(errors.Any())
                Console.WriteLine("FAILED!!!\n\t" + string.Join("\n\t",errors.ToArray()));

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            BuildWebHost(args, tethysConfig).Run();
        }

        public static IWebHost BuildWebHost(string[] args, TethysConfig config) =>

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:" + config.HttpPort)
                .Build();
    }
}
