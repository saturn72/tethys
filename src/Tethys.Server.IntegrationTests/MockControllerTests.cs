using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace Tethys.Server.IntegrationTests
{
    public class MockControllerTests //: IClassFixture<TethysServerWebApplicationFactory>
    {
        public MockControllerTests()
        {
            var factory = new TethysServerWebApplicationFactory();
            Client = factory.CreateClient();
        }
        #region HttpPost to push Uri
        private readonly HttpClient Client;
        const string pushUri = "tethys/api/mock/push";

        [Fact]
        public async Task PushSingleNotificationAsync()
        {
            var notifications = new object[] {
            new {
                key = "notification-key",
                body = "notification-body"
            }
            };
            var request = BuildHttpRequestMessage(notifications);
            var response = await Client.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            throw new System.NotImplementedException("listen to web socket evebnt");
        }

        [Fact]
        public async Task PushSingleNotificationWithDelayAsync()
        {
            var notifications = new object[]{
            new {
                key = "notification-key",
                body = "notification-body",
                delay = 3000,//will be delayed for 3000 milisecs
            }};
            var request = BuildHttpRequestMessage(notifications);
            var response = await Client.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            throw new System.NotImplementedException("listen to web socket evebnt");
        }

        [Fact]
        public async Task PushSingleNotificationRecuringAsync()
        {
            var notifications = new object[]{
            new{
                key = "notification-key",
                body = "notification-body",
                NotifyTimes = 3,//will be notified for 3 times
            }
            };
            var request = BuildHttpRequestMessage(notifications);
            var response = await Client.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            throw new System.NotImplementedException("listen to web socket evebnt");
        }

        [Fact]
        public async Task PushMultipleNotificationAsync()
        {
            var notifications = new object[]{
                //notified immediately 
                new{
                key = "notification-key",
                body="notification-body"
            },
                // delayed for 300 milisecs
                new{
                key = "notification-key",
                body="notification-body",
                delay=3000,
            },
            //recurring notification - notified 3 times with 500 milisecs delay
             new{
                key = "notification-key",
                body="notification-body",
                delay = 500,
                NotifyTimes=3,//will be notified for 3 times
            }
            };
            var request = BuildHttpRequestMessage(notifications);
            var response = await Client.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            throw new System.NotImplementedException("listen to web socket evebnt");
        }
        #endregion

        #region Utilities

        private HttpRequestMessage BuildHttpRequestMessage(object notifications)
        {
            var json = JsonConvert.SerializeObject(notifications, Formatting.None); ;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, pushUri)
            {
                Content = content,
            };

            return req;
        }

        #endregion
    }
}
