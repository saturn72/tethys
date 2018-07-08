using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tethys.Server.Hubs;

namespace Tethys.Server.Services
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<MockHub> _mockHub;

        public NotificationPublisher(IHubContext<MockHub> mockHub)
        {
            _mockHub = mockHub;
        }
        public async Task ToAll(string notificationKey, string notificationBody)
        {
            await _mockHub.Clients.All.SendAsync(notificationKey, notificationBody);
        }
    }
}