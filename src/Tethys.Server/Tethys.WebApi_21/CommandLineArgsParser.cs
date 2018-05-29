using System;
using System.Collections.Generic;
using System.Linq;

namespace Tethys.WebApi
{
    public class CommandLineArgsParser
    {
        private const string HttpPorts = "--httpPort";

        public static TethysConfig Parse(IEnumerable<string> args, ICollection<string> errors)
        {
            var argsList = args.ToList();

            Console.WriteLine("Parsing Command line arguments.");
            Console.WriteLine("For more details see: " + Consts.SiteUrl);

            var clad = new[]
            {
                new CommandLineArgsData(HttpPorts, false),
            };
            foreach (var c in clad)
            {
                var argsListCount = argsList.Count;
                for (var i = 0; i < argsListCount; i++)
                {
                    if (!argsList.ElementAt(i).ToLower().Equals(c.Key, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    c.Value = argsList[i + 1].Trim();
                    argsList.RemoveAt(i);   //remove key
                    argsList.RemoveAt(i); //remove value
                    break;  //move to next element
                }
            }
            return BuildTethysConfig(clad);
        }

        private static TethysConfig BuildTethysConfig(IEnumerable<CommandLineArgsData> commandLineArgsDatas)
        {
            var config = TethysConfig.Default;
            var httpPortData = commandLineArgsDatas.FirstOrDefault(c => c.Key.Equals(HttpPorts, StringComparison.InvariantCultureIgnoreCase));
            if (httpPortData?.Value != null)
                config.HttpPort = short.Parse(httpPortData.Value);

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