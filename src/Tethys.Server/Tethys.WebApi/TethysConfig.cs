using System.Collections.Generic;

namespace Tethys.WebApi
{
    public class TethysConfig
    {
        public IEnumerable<ushort> HttpPorts { get; set; }
        public string WebSocketSuffix { get; set; }
        public static TethysConfig Default =>
            new TethysConfig
            {
                HttpPorts = new ushort[]{4880, 5645},
                WebSocketSuffix = "ws"
            };
    }
}