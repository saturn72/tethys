using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ServiceStack.Text;
using Tethys.TestFramework.Commands;
using Xunit.Abstractions;

namespace Tethys.TestFramework
{
    public abstract class WebTestBase : IDisposable
    {
        private readonly string _tethysServerUrl;

        #region Fields

        private HttpClient _httpClient;
        private static IWebDriver _webDriver;

        #endregion

        #region Properties

        private HttpClient TettysHttpClient => _httpClient ?? (_httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://" + _tethysServerUrl),
        });

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
            await SendPostRequestToTethys(httpCall, "setup");
        }

        protected void SendPushNotifications(IEnumerable<PushNotification> pushNotifications)
        {
            SendPostRequestToTethys(pushNotifications, "mock/push");
        }

        private async Task SendPostRequestToTethys(object httpCall, string resource)
        {
            var content = JsonSerializer.SerializeToString(httpCall);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            //send seup request to tethys 
            var body = new ByteArrayContent(contentBytes);
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
