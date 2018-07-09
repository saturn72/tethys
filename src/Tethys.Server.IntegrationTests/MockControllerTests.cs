using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace Tethys.Server.IntegrationTests
{
    public class MockControllerTests : IntegrationTestBase
    {
        #region HttpPost to push Uri
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
            var request = BuildHttpRequestMessage(notifications, pushUri);
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
            var request = BuildHttpRequestMessage(notifications, pushUri);
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
            var request = BuildHttpRequestMessage(notifications, pushUri);
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
            var request = BuildHttpRequestMessage(notifications, pushUri);
            var response = await Client.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            throw new System.NotImplementedException("listen to web socket evebnt");
        }
        #endregion
    }
}
