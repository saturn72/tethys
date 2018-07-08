using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tethys.WebApi.Tests
{
    public class PushNotificationTests : IntegrationTestBase
    {
        public PushNotificationTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public void PushSingleMultipleNotifications()
        {
            var notifications = new[]{
                new{

                }
            };
            throw new System.NotImplementedException("");
        }
    }
}