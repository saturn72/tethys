using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ServiceStack.Text;
using Tethys.TestFramework.Models;
using Xunit.Abstractions;

namespace Tethys.TestFramework
{
    public abstract class WebTestBase : IDisposable
    {
        private readonly string _tethysServerUrl;

        #region Fields

        private HttpClient _httpClient;
        private static IWebDriver _webDriver;
        private ClientWebSocket _clientWebSocket;

        #endregion

        #region Properties

        private HttpClient TettysHttpClient => _httpClient ?? (_httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://" + _tethysServerUrl),
        });

        //protected ClientWebSocket TethysWebSocketClient =>
        //    _clientWebSocket ?? (_clientWebSocket = CreatewebSocketClient());

        //private ClientWebSocket CreatewebSocketClient()
        //{
        //    var ws = new ClientWebSocket();
        //    ws.ConnectAsync(new Uri("ws"), )
        //    ws.Options.
        //    {
        //        Options
        //    }
        //    ws.

        //    var wsc = new WebSocketClient()
        //var Buffersize = 1024 * 4;

        //    var buffer = WebSocket.CreateClientBuffer(Buffersize, Buffersize);
        //    ClientWebSocket.
        //    throw new NotImplementedException();
        //}
        /*
         * 

        private StringBuilder _webSocketLog;

        private async Task LogIncomingWebSocketData(WebSocket clientWebSocket, CancellationToken cToken)
        {
            if (_webSocketLog == null)
                _webSocketLog = new StringBuilder();

            var buffer = new byte[Buffersize];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var inMessage = Encoding.UTF8.GetString(buffer);
                    _webSocketLog.AppendLine(inMessage);
                    //                    _outputHelper.WriteLine(inMessage);
                }
            }
        }

         */
        protected static IWebDriver WebDriver
        {
            get
            {
                if (_webDriver == null)
                    BuildWebDriver();
                return _webDriver;

                void BuildWebDriver()
                {
                    var co = new ChromeOptions();
                    co.AddArgument("--start-maximized");
                    _webDriver = new ChromeDriver(".", co);
                }
            }
        }

        #endregion
        #region Ctor
        protected WebTestBase(ITestOutputHelper outputHelper= null, string tethysServerUrl = "localhost:4880")
        {
            foreach (var cdp in Process.GetProcessesByName("chromedriver.exe"))
                cdp.Kill();
            _tethysServerUrl = tethysServerUrl;

        }

        #endregion

        protected virtual async Task MockHttpRequest(object httpCall)
        {
            await SendPostRequestToTethys("mock/setup", new[]{httpCall});
        }

        protected void SendPushNotifications(IEnumerable<PushNotification> pushNotifications)
        {
            SendPostRequestToTethys("mock/push", pushNotifications);
        }

        private async Task SendPostRequestToTethys(string resource, object content)
        {
            var contentAsString = JsonSerializer.SerializeToString(content);
            //var contentBytes = Encoding.UTF8.GetBytes(contentAsString);
            ////send seup request to tethys 
            //var body = new ByteArrayContent(contentBytes);
            var body = new StringContent(contentAsString, Encoding.UTF8);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var setupHttpRequest =
                new HttpRequestMessage(HttpMethod.Post, new Uri(TettysHttpClient.BaseAddress, "tethys/api/" + resource))
                {
                    Content = body
                };
            var setupResponse = await TettysHttpClient.SendAsync(setupHttpRequest);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
