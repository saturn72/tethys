using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Tethys.Server;
using Xunit;

namespace Tethys.Server.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<TethysServerWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected WebSocket WebSocket;
        protected ArraySegment<byte> WebSocketBuffer;
        public IntegrationTestBase()
        {
            var factory = new TethysServerWebApplicationFactory();
            Client = factory.CreateClient();
            InitWebSocket(factory.Server);
        }
        public void InitWebSocket(TestServer server)
        {
            var wsc = server.CreateWebSocketClient();
            var uri = new Uri(server.BaseAddress, Consts.TethysWebSocketPath);
            WebSocket = wsc.ConnectAsync(uri, CancellationToken.None).Result;
            WebSocketBuffer = WebSocket.CreateClientBuffer(1024, 1024);
            Task.Run(() => WebSocket.ReceiveAsync(WebSocketBuffer, CancellationToken.None));
        }
        #region Utilities

        protected HttpRequestMessage BuildHttpRequestMessage(object notifications, string uri)
        {
            var json = JsonConvert.SerializeObject(notifications, Formatting.None); ;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content,
            };

            return req;
        }

        #endregion
    }
}