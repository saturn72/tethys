using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Tethys.Samples
{
    public class Examples
    {
        public Examples()
        {
            //for test purpose only
            foreach (var cdp in Process.GetProcessesByName("chromedriver.exe"))
                cdp.Kill();
        }
        [Fact]
        public async Task NotFoundExample()
        {
            //start tethys.webapi process if required

            HttpClient testHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:4880"), //set the server Uri
            };

            //TODO: listen to server output via web socket

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
                BaseAddress = new Uri("http://localhost:4880"), //set the server Uri
            };

            //TODO: listen to server output via web socket

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
                    Delay = 5000,
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
    }
}
