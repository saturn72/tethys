using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tethys.Server.Hubs;

namespace Tethys.Server.Services.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<MockHub> _mockHub;
        private readonly IHubContext<TethysHub> _tethysHub;

        public NotificationPublisher(IHubContext<MockHub> mockHub, IHubContext<TethysHub> tethysHub)
        {
            _mockHub = mockHub;
            _tethysHub = tethysHub;
        }
        public async Task ToServerUnderTestClients(string notificationKey, object notificationBody)
        {
            await _mockHub.Clients.All.SendAsync(notificationKey, notificationBody);
            await ToLogClients(notificationKey, notificationBody);
        }

        public async Task ToLogClients(string notificationKey, object notificationBody)
        {
            await _tethysHub.Clients.All.SendAsync(Consts.PushNotificationLog, notificationKey, notificationBody);
        }
    }
}