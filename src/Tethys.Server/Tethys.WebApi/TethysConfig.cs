using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Tethys.WebApi
{
    public class TethysConfig
    {
        public IEnumerable<ushort> HttpPorts { get; set; }
        public IEnumerable<string> WebSocketSuffix { get; set; }

        public static TethysConfig Default =>
            new TethysConfig
            {
                HttpPorts = new ushort[] {4880},
                WebSocketSuffix = new[] {"ws"}
            };

        public static TethysConfig FromConfiguration(IConfiguration configuration)
        {
            return new TethysConfig
            {
                HttpPorts = GetConfigurationValues(configuration, "tethysConfig:httpPorts").Select(ushort.Parse),
                WebSocketSuffix = GetConfigurationValues(configuration, "tethysConfig:webSocketSuffix")
            };
        }

        private static IEnumerable<string> GetConfigurationValues(IConfiguration configuration, string jsonPath)
        {
            return configuration.GetSection(jsonPath).GetChildren()
                .Select(c => c.Value.Trim());
        }
    }
}