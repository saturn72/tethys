using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using Xunit;
using Xunit.Abstractions;

namespace Tethys.Samples
{
    public class Examples:IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        private static ClientWebSocket _clientWebSocket;
        private const int Buffersize = 1024 * 4;
        public Examples(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            //for test purpose only
            foreach (var cdp in Process.GetProcessesByName("chromedriver.exe"))
                cdp.Kill();
            
            _clientWebSocket = new ClientWebSocket();
            _clientWebSocket.ConnectAsync(new Uri("ws://" + BaseAddress + "/tethys/ws"), CancellationToken.None);

            _cts = new CancellationTokenSource();
            var thread = new Thread(async () => await LogIncomingWebSocketData(_clientWebSocket, _cts.Token));
            thread.Start();
        }

        #region Consts 

        private static readonly Uri BaseAddress = new Uri("localhost:4880");

        #endregion

        [Fact]
        public async Task NotFoundExample()
        {
            //start tethys.webapi process if required

            HttpClient testHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://" + BaseAddress), //set the server Uri
            };

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

            var content = JsonConvert.SerializeObject(httpCall);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            //send seup request to tethys 
            var body = new ByteArrayContent(contentBytes);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var setupHttpRequest =
                new HttpRequestMessage(HttpMethod.Post, new Uri(testHttpClient.BaseAddress, "tethys/api/setup"))
                {
                    Content = body
                };
            var setupResponse = await testHttpClient.SendAsync(setupHttpRequest);

            //Start tests
            var co = new ChromeOptions();
            co.AddArgument("--start-maximized");

            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            using (var driver = new ChromeDriver(".", co))
            {
                driver.Navigate().GoToUrl(indexUri.AbsoluteUri);
                var userIdElem = driver.FindElementByName("user-id");
                userIdElem.SendKeys("1");

                var submitElem = driver.FindElementByName("submit");
                submitElem.Click();

                var userDetailsElem = driver.FindElementByName("user-details");

                var actualUserDetails = userDetailsElem.GetAttribute("value");
                Assert.Equal(expectedUserDetails, actualUserDetails);

                //assert bg color
                var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
                Assert.Equal("rgba(255, 165, 0, 1)", userDetailsBgColor);
            }
        }

        [Fact]
        public async Task TimeoutExample()
        {
            //start tethys.webapi process if required

            HttpClient testHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://" + BaseAddress), //set the server Uri
            };

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

            var content = JsonConvert.SerializeObject(httpCall);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            //send seup request to tethys 
            var body = new ByteArrayContent(contentBytes);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var setupHttpRequest =
                new HttpRequestMessage(HttpMethod.Post, new Uri(testHttpClient.BaseAddress, "tethys/api/setup"))
                {
                    Content = body
                };
            var setupResponse = await testHttpClient.SendAsync(setupHttpRequest);

            //Start tests
            var co = new ChromeOptions();
            co.AddArgument("--start-maximized");

            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            using (var driver = new ChromeDriver(".", co))
            {
                driver.Navigate().GoToUrl(indexUri.AbsoluteUri);
                var userIdElem = driver.FindElementByName("user-id");
                userIdElem.SendKeys("1");

                var submitElem = driver.FindElementByName("submit");
                submitElem.Click();
                Thread.Sleep(10000); //wait for timeout
                var userDetailsElem = driver.FindElementByName("user-details");

                //assert bg color
                var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
                Assert.Equal("rgba(255, 192, 203, 1)", userDetailsBgColor);

                //assert message
                var actualUserDetails = userDetailsElem.GetAttribute("value");
                Assert.Contains("Timeout error message", actualUserDetails);
            }
        }


        [Fact]
        public async Task OkResponseExample()
        {
            //start tethys.webapi process if required

            HttpClient testHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://" + BaseAddress), //set the server Uri
            };

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

            var content = JsonConvert.SerializeObject(httpCall);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            //send seup request to tethys 
            var body = new ByteArrayContent(contentBytes);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var setupHttpRequest =
                new HttpRequestMessage(HttpMethod.Post, new Uri(testHttpClient.BaseAddress, "tethys/api/setup"))
                {
                    Content = body
                };
            var setupResponse = await testHttpClient.SendAsync(setupHttpRequest);

            //Start tests
            var co = new ChromeOptions();
            co.AddArgument("--start-maximized");

            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            using (var driver = new ChromeDriver(".", co))
            {
                driver.Navigate().GoToUrl(indexUri.AbsoluteUri);
                var userIdElem = driver.FindElementByName("user-id");
                userIdElem.SendKeys("1");

                var submitElem = driver.FindElementByName("submit");
                submitElem.Click();
                Thread.Sleep(1000); //wait for timeout
                var userDetailsElem = driver.FindElementByName("user-details");

                //assert bg color
                var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
                Assert.Equal("rgba(173, 216, 230, 1)", userDetailsBgColor);

                //assert message
                var actualUserDetails = userDetailsElem.GetAttribute("value");
                Assert.Contains(expUserDetails, actualUserDetails);
            }
        }

        [Fact]
        public async Task ConnectToServerNotificationsViaWebSocketsExample()
        {
            //start tethys.webapi process if required

            //Configure mock server via Http Calls            
            HttpClient testHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://" + BaseAddress), //set the server Uri
            };
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

            var content = JsonConvert.SerializeObject(httpCall);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            //send seup request to tethys 
            var body = new ByteArrayContent(contentBytes);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var setupHttpRequest =
                new HttpRequestMessage(HttpMethod.Post, new Uri(testHttpClient.BaseAddress, "tethys/api/setup"))
                {
                    Content = body
                };
            var setupResponse = await testHttpClient.SendAsync(setupHttpRequest);

            //Start tests
            var co = new ChromeOptions();
            co.AddArgument("--start-maximized");

            var indexUri = new System.Uri(Environment.CurrentDirectory + @"/app-under-test/index.html");
            using (var driver = new ChromeDriver(".", co))
            {
                driver.Navigate().GoToUrl(indexUri.AbsoluteUri);
                var userIdElem = driver.FindElementByName("user-id");
                userIdElem.SendKeys("1");

                var submitElem = driver.FindElementByName("submit");
                submitElem.Click();
                Thread.Sleep(1000); //wait for timeout
                var userDetailsElem = driver.FindElementByName("user-details");

                //assert bg color
                var userDetailsBgColor = userDetailsElem.GetCssValue("background-color");
                Assert.Equal("rgba(173, 216, 230, 1)", userDetailsBgColor);

                //assert message
                var actualUserDetails = userDetailsElem.GetAttribute("value");
                Assert.Contains(expUserDetails, actualUserDetails);
            }
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
                    _outputHelper.WriteLine(inMessage);
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
        }
    }
}
