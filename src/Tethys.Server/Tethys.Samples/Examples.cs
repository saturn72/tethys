using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using Tethys.TestFramework;
using Xunit;
using Xunit.Abstractions;

namespace Tethys.Samples
{
    public class Examples : WebTestBase
    {
        private static readonly Uri AutUri = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
        private static ClientWebSocket _clientWebSocket;
        private const int Buffersize = 1024 * 4;
        //public Examples()
        //{
        //    //_outputHelper = outputHelper;
        //    ////for test purpose only


        //    //_clientWebSocket = new ClientWebSocket();
        //    //_clientWebSocket.ConnectAsync(new Uri("ws://" + TethysServerUrl + "/tethys/ws"), CancellationToken.None);

        //    //_cts = new CancellationTokenSource();
        //    //var thread = new Thread(async () => await LogIncomingWebSocketData(_clientWebSocket, _cts.Token));
        //    //thread.Start();
        //}

        [Fact]
        public async Task NotFoundExample()
        {
            //Mock next request
            const int userId = 1;
            var expectedUserDetails = "{userId:\'" + userId + "\',message:\'user id not found\'}";
            object httpCall = new
            {
                //Expected incoming request
                Request = new
                {
                    Resource = "/api/users/" + userId,
                    HttpMethod = "GET"
                },
                //what the server should return
                Response = new
                {
                    HttpstatusCode = StatusCodes.Status404NotFound,
                    Body = expectedUserDetails,
                }
            };
            await MockHttpRequest(httpCall);

            //Start tests
            var applicationPath = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            WebDriver.Navigate().GoToUrl(applicationPath.AbsoluteUri);
            var userIdElem = WebDriver.FindElementByName("user-id");
            userIdElem.SendKeys("1");

            var submitElem = WebDriver.FindElementByName("submit");
            submitElem.Click();

            var userDetailsElem = WebDriver.FindElementByName("user-details");

            var actualUserDetails = userDetailsElem.GetAttribute("value");
            Assert.Equal(expectedUserDetails, actualUserDetails);

            //assert bg color
            var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
            Assert.Equal("rgba(255, 165, 0, 1)", userDetailsBgColor);
        }

        [Fact]
        public async Task TimeoutExample()
        {
            const int userId = 1;
            object httpCall = new
            {
                //Expected incoming request
                Request = new
                {
                    Resource = "/api/users/" + userId,
                    HttpMethod = "GET"
                },
                //what the server should return
                Response = new
                {
                    Delay = 3000,
                    HttpstatusCode = StatusCodes.Status200OK,
                }
            };
            await MockHttpRequest(httpCall);

            //Start tests

            WebDriver.Navigate().GoToUrl(AutUri.AbsoluteUri);
            var userIdElem = WebDriver.FindElementByName("user-id");
            userIdElem.SendKeys("1");

            var submitElem = WebDriver.FindElementByName("submit");
            submitElem.Click();
            Thread.Sleep(3000); //wait for timeout
            var userDetailsElem = WebDriver.FindElementByName("user-details");

            //assert bg color
            var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
            Assert.Equal("rgba(255, 192, 203, 1)", userDetailsBgColor);

            //assert message
            var actualUserDetails = userDetailsElem.GetAttribute("value");
            Assert.Contains("Timeout error message", actualUserDetails);
        }


        [Fact]
        public async Task OkResponseExample()
        {
            const int userId = 1;
            var expUserDetails = "{\'first_name\":\"Roi\",\"last_name\":\"shabtai\"}";
            object httpCall = new
            {
                //Expected incoming request
                Request = new
                {
                    Resource = "/api/users/" + userId,
                    HttpMethod = "GET"
                },
                //what the server should return
                Response = new
                {
                    HttpstatusCode = StatusCodes.Status200OK,
                    Body = expUserDetails,
                }
            };
            await MockHttpRequest(httpCall);
            //Start tests
            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            WebDriver.Navigate().GoToUrl(indexUri.AbsoluteUri);
            var userIdElem = WebDriver.FindElementByName("user-id");
            userIdElem.SendKeys("1");

            var submitElem = WebDriver.FindElementByName("submit");
            submitElem.Click();
            Thread.Sleep(1000); //wait for timeout
            var userDetailsElem = WebDriver.FindElementByName("user-details");

            //assert bg color
            var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
            Assert.Equal("rgba(173, 216, 230, 1)", userDetailsBgColor);

            //assert message
            var actualUserDetails = userDetailsElem.GetAttribute("value");
            Assert.Contains(expUserDetails, actualUserDetails);
        }

        [Fact]
        public async Task ReceivePushNotifications()
        {
            const int userId = 1;
            var expUserDetails = "{\'first_name\":\"Roi\",\"last_name\":\"shabtai\"}";
            object httpCall = new
            {
                //Expected incoming request
                Request = new
                {
                    Resource = "/api/users/" + userId,
                    HttpMethod = "GET"
                },
                //what the server should return
                Response = new
                {
                    HttpstatusCode = StatusCodes.Status200OK,
                    Body = expUserDetails,
                }
            };
            await MockHttpRequest(httpCall);
            var pushNotifications = new List<object>();
            var random = new Random(123);
            for (var i = 0; i < 5; i++)
                pushNotifications.Add(
                    new
                    {
                        Key = "ReceiveMessage",
                        Delay = random.Next(150, 3000),
                        Body = "this is notification #" + i
                    });
            SendPushNotifications(pushNotifications);

            //Start tests
            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            WebDriver.Navigate().GoToUrl(indexUri.AbsoluteUri);
            var userIdElem = WebDriver.FindElementByName("user-id");
            userIdElem.SendKeys("1");

            var submitElem = WebDriver.FindElementByName("submit");
            submitElem.Click();
            Thread.Sleep(1000); //wait for timeout
            var userDetailsElem = WebDriver.FindElementByName("user-details");

            //assert bg color
            var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
            Assert.Equal("rgba(173, 216, 230, 1)", userDetailsBgColor);

            //assert message
            var actualUserDetails = userDetailsElem.GetAttribute("value");
            Assert.Contains(expUserDetails, actualUserDetails);
        }

        private StringBuilder _webSocketLog;
        private readonly CancellationTokenSource _cts;

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

        public void Dispose()
        {
            Thread.Sleep(10000);
            //Dispose ClientWebsockets
            _cts.Cancel();
            _clientWebSocket.Abort();
            _clientWebSocket.Dispose();
            base.Dispose();
        }
    }
}
