using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Tethys.WebApi
{
    public class CommandLineArgsParser
    {
        private const string HttpPorts = "--httpPort";
        private const string ConfigFile = "--config";

        public static IConfiguration Load(IEnumerable<string> args, ICollection<string> errors)
        {
            var argsList = args.ToList();

            Console.WriteLine("Parsing Command line arguments.");
            Console.WriteLine("For more details see: " + Consts.SiteUrl);

            var clad = new[]
            {
                new CommandLineArgsData(HttpPorts, false),
                new CommandLineArgsData(ConfigFile, false),
            };
            foreach (var c in clad)
            {
                for (var i = 0; i < argsList.Count; i++)
                {
                    if (!argsList.ElementAt(i).ToLower().Equals(c.Key, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    c.Value = argsList[i + 1].Trim();
                    //remove "
                    if (c.Value.StartsWith('\"')) c.Value = c.Value.Substring(1, c.Value.Length);
                    if (c.Value.EndsWith('\"')) c.Value = c.Value.Substring(0, c.Value.Length - 1);
                    argsList.RemoveAt(i);   //remove key and 
                    argsList.RemoveAt(i--); //remove value
                }
            }
            return BuildTethysConfig(clad);
        }

        private static IConfiguration BuildTethysConfig(IEnumerable<CommandLineArgsData> commandLineArgsDatas)
        {
            var tethysConfig = LoadValuesFromCommandLine(commandLineArgsDatas);

            var configDirectory = Path.GetDirectoryName(tethysConfig.ConfigFile);
            if (configDirectory.Length == 0)
                configDirectory = Directory.GetCurrentDirectory();

                //Now overide using config
            var builder = new ConfigurationBuilder()
                .SetBasePath(configDirectory)
                .AddJsonFile(tethysConfig.ConfigFile);
            var configuration = builder.Build();

            tethysConfig.HttpPorts = configuration.GetSection("tethysConfig:httpPorts")?.GetChildren()?
                                   .Select(hp => ushort.Parse(hp.Value)) ?? tethysConfig.HttpPorts;

            tethysConfig.HttpsPorts = configuration.GetSection("tethysConfig:httpsPorts")?.GetChildren()?
                 .Select(hsp => ushort.Parse(hsp.Value)) ?? tethysConfig.HttpsPorts;

            tethysConfig.WebSocketSuffix = configuration.GetSection("tethysConfig:webSocketSuffix")?.GetChildren()?
                .Select(ws => ws.Value) ?? tethysConfig.WebSocketSuffix;

            return configuration;
            //return tethysConfig;

        }

        private static TethysConfig LoadValuesFromCommandLine(IEnumerable<CommandLineArgsData> commandLineArgsDatas)
        {
            var config = TethysConfig.Default;
            var httpPortData =
                commandLineArgsDatas.FirstOrDefault(c => c.Key.Equals(HttpPorts, StringComparison.InvariantCultureIgnoreCase));
            if (httpPortData != null && !string.IsNullOrEmpty(httpPortData.Value) &&
                !string.IsNullOrWhiteSpace(httpPortData.Value))
                config.HttpPorts = httpPortData.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ushort.Parse);

            var configFile = commandLineArgsDatas
                .FirstOrDefault(c => c.Key.Equals(ConfigFile, StringComparison.InvariantCultureIgnoreCase))?.Value;

            if (!string.IsNullOrEmpty(configFile) || !string.IsNullOrWhiteSpace(configFile))
                config.ConfigFile = configFile;
            return config;
        }

        public class CommandLineArgsData
        {
            #region CTOR

            public CommandLineArgsData(string key, bool required)
            {
                Key = key.ToLower();
                Required = required;
            }

            #endregion

            #region Properties

            internal string Key { get; }
            internal bool Required { get; }
            internal string Value { get; set; }

            #endregion
        }
    }
}