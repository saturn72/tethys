using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Tethys.Server;
using Xunit;

namespace Tethys.Server.IntegrationTests
{
    public abstract class IntegrationTestBase// : IClassFixture<TethysServerWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        public IntegrationTestBase()
        {
            // var factory = new TethysServerWebApplicationFactory();
            // Client = factory.CreateClient();
        }
    }
}