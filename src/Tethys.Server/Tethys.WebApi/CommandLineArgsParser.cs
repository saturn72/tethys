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
                for (var i = 0; i < argsList.Count; i++)
                {
                    if (!argsList.ElementAt(i).ToLower().Equals(c.Key, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    c.Value = argsList[i + 1].Trim();
                    //remove "
                    if (c.Value.StartsWith('\"')) c.Value = c.Value.Substring(1, c.Value.Length);
                    if (c.Value.EndsWith('\"')) c.Value = c.Value.Substring(0, c.Value.Length-1);
                    argsList.RemoveAt(i);   //remove key and 
                    argsList.RemoveAt(i--); //remove value
                }
            }
            return BuildTethysConfig(clad);
        }

        private static TethysConfig BuildTethysConfig(IEnumerable<CommandLineArgsData> commandLineArgsDatas)
        {
            var config = TethysConfig.Default;
            var httpPortData = commandLineArgsDatas.FirstOrDefault(c => c.Key.Equals(HttpPorts, StringComparison.InvariantCultureIgnoreCase));
            if (httpPortData!=null && !string.IsNullOrEmpty(httpPortData.Value) && !string.IsNullOrWhiteSpace(httpPortData.Value))
                config.HttpPorts = httpPortData.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ushort.Parse);

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