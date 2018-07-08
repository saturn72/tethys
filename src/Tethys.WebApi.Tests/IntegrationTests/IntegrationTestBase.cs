using System;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tethys.WebApi.Tests
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Tethys.WebApi.Startup>>
    {
        private readonly WebApplicationFactory<Tethys.WebApi.Startup> _factory;
        protected readonly HttpClient HttpClient;
        protected IntegrationTestBase(WebApplicationFactory<Tethys.WebApi.Startup> factory)
        {
            _factory = factory;
            HttpClient = _factory.CreateClient();
        }
    }
}
