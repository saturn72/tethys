using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Tethys.Server;
using Xunit;

namespace Tethys.Server.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<TethysServerWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        public IntegrationTestBase()
        {
            var factory = new TethysServerWebApplicationFactory();
            Client = factory.CreateClient();
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