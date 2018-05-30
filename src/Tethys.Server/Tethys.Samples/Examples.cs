using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using Tethys.TestFramework;
using Tethys.TestFramework.Commands;
using Xunit;

namespace Tethys.Samples
{
    public class Examples : WebTestBase
    {
        private static readonly Uri AutUri = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
        private const int Buffersize = 1024 * 4;

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
            var indexUri = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
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
        public void ReceiveAsyncPushNotifications()
        {
            var indexUri = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            WebDriver.Navigate().GoToUrl(indexUri.AbsoluteUri);

            var pushNotifications = new List<PushNotification>();
            var random = new Random(123);
            for (var i = 0; i < 5; i++)
                pushNotifications.Add(
                    new PushNotification
                    {
                        Key = "ReceiveMessage",
                        Delay = random.Next(150, 3000),
                        Body = "this is notification #" + i
                    });
            SendPushNotifications(pushNotifications);


            Thread.Sleep(pushNotifications.Select(n => n.Delay).Sum());
            //Start tests
            var notifications = WebDriver.FindElementByName("push-notify");
            var value = notifications.GetAttribute("value");
            Assert.Contains("this is notification #", value);
        }

        [Fact]
        public void ReceiveMultiAsyncPushNotifications()
        {
            var indexUri = new Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            WebDriver.Navigate().GoToUrl(indexUri.AbsoluteUri);

            var pushNotifications1 = new List<PushNotification>();
            var random = new Random(123);
            for (var i = 0; i < 5; i++)
                pushNotifications1.Add(
                    new PushNotification
                    {
                        Key = "ReceiveMessage",
                        Delay = random.Next(150, 1000),
                        Body = "Type 1 notification #" + i
                    });
            SendPushNotifications(pushNotifications1);

            var pushNotifications2 = new List<PushNotification>();
            for (var i = 0; i < 5; i++)
                pushNotifications2.Add(
                    new PushNotification
                    {
                        Key = "ReceiveMessageType2",
                        Delay = random.Next(150, 3000),
                        Body = "Type 2 notification #" + i
                    });
            SendPushNotifications(pushNotifications2);

            var sleepTime = Math.Max(pushNotifications1.Select(n => n.Delay).Sum(),
                pushNotifications2.Select(n => n.Delay).Sum());
            Thread.Sleep(sleepTime);
            //Start tests
            var notifications1 = WebDriver.FindElementByName("push-notify");
            var value1 = notifications1.GetAttribute("value");
            Assert.Contains("Type 1 notification #", value1);

            var notifications2 = WebDriver.FindElementByName("push-notify-2");
            var value2 = notifications2.GetAttribute("value");
            Assert.Contains("Type 2 notification #", value2);
        }


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
    }
}
