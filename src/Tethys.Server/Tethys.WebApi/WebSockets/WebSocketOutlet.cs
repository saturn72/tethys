using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tethys.WebApi.WebSockets
{
    public class WebSocketOutlet
    {
        private static readonly List<byte> Buffer = new List<byte>();
        private static readonly ICollection<WebSocket> WebSockets = new List<WebSocket>();
        private static bool _publishing;

        public static async Task Send(WebSocket ws)
        {
            WebSockets.Add(ws);
            if (!_publishing)
                await Publish();
        }
        public static async Task Publish()
        {
            while (WebSockets.Any())
            {
                _publishing = true;
            
                if (!Buffer.Any())
                {
                    Thread.Sleep(150);
                    continue;
                }

                var array = Buffer.ToArray();
                var bufferCount = Buffer.Count;
                var toremove = new List<WebSocket>();
                await Task.Run(() =>
                {
                    foreach (var ws in WebSockets)
                    {
                        try
                        {
                            if (!ws.CloseStatus.HasValue)
                            {
                                ws.SendAsync(
                                    new ArraySegment<byte>(array, 0, bufferCount),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                            else
                            {
                                toremove.Add(ws);
                            }
                        }
                        catch (ObjectDisposedException ex)
                        {
                            toremove.Add(ws);
                        }
                    }
                });

                Buffer.Clear();
                foreach (var trws in toremove)
                    WebSockets.Remove(trws);
            }
            _publishing = false;
        }

        public static void AddToBuffer(string data)
        {
            Buffer.AddRange(Encoding.UTF8.GetBytes(data));
        }
    }
}
