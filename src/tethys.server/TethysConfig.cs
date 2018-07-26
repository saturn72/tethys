using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Tethys.Server
{
    public class TethysConfig
    {
        public IEnumerable<ushort> HttpPorts { get; set; }
        public IEnumerable<ushort> HttpsPorts { get; set; }
        public IEnumerable<string> WebSocketSuffix { get; set; }
        public string ConfigFile { get; set; }

        public static TethysConfig Default =>
            new TethysConfig
            {
                HttpPorts = new ushort[] { 4880 },
                HttpsPorts = new ushort[] { 4881 },
                WebSocketSuffix = new[] { "ws" },
                ConfigFile = "appsettings.json",
            };


        public static TethysConfig FromConfiguration(IConfiguration configuration)
        {
            var curMode = GetConfigurationValues(configuration, "tethysConfig:mode").FirstOrDefault();

            return new TethysConfig
            {
                HttpPorts = GetConfigurationValues(configuration, "tethysConfig:httpPorts").Select(ushort.Parse),
                WebSocketSuffix = GetConfigurationValues(configuration, "tethysConfig:webSocketSuffix"),
            };
        }

        private static IEnumerable<string> GetConfigurationValues(IConfiguration configuration, string jsonPath)
        {
            return configuration.GetSection(jsonPath).GetChildren()
                .Select(c => c.Value.Trim());
        }
    }
}