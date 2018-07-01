using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tethys.WebApi.Hubs;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Services
{
    public class NotificationService : INotificationService
    {
        private static CancellationTokenSource _notificationsCancelationTokenSource =
            new CancellationTokenSource();

        private readonly IHubContext<MockHub> _mockHub;

        public NotificationService(IHubContext<MockHub> mockHub)
        {
            _mockHub = mockHub;
        }

        public async Task Notify(IEnumerable<PushNotification> notifications)
        {
            //stop previous notifications
            Stop();
            //recreate cancelation token source
            _notificationsCancelationTokenSource = new CancellationTokenSource();
            var ct = _notificationsCancelationTokenSource.Token;
            await Task.Run(() =>
            {
                foreach (var notification in notifications)
                {
                    if (ct.IsCancellationRequested)
                        return;
                    Thread.Sleep(notification.Delay);
                    if (ct.IsCancellationRequested)
                        return;
                    _mockHub.Clients.All.SendAsync(notification.Key, notification.Body);
                }
            }, ct);
        }

        public void Stop()
        {
            _notificationsCancelationTokenSource?.Cancel();
        }
    }
}