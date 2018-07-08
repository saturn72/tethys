using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.Hubs;
using Tethys.Server.Models;

namespace Tethys.Server.Services
{
    public class NotificationService : INotificationService
    {
        private static CancellationTokenSource _notificationsCancelationTokenSource =
            new CancellationTokenSource();

        private readonly IHubContext<MockHub> _mockHub;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(IHubContext<MockHub> mockHub, INotificationRepository notificationRepository)
        {
            _mockHub = mockHub;
            _notificationRepository = notificationRepository;
        }

        public async Task NotifyAsync(IEnumerable<PushNotification> notifications)
        {
            //stop previous notifications
            Stop();

            //setup database
            foreach (var n in notifications)
            {
                n.NotifyTimes = Math.Max(1, n.NotifyTimes);
                n.NotifiedCounter = 0;
            }
            _notificationRepository.Create(notifications);

            //recreate cancelation token source
            _notificationsCancelationTokenSource = new CancellationTokenSource();
            var ct = _notificationsCancelationTokenSource.Token;
            await Task.Run(() =>
            {
                foreach (var notification in notifications)
                {
                    while (!notification.WasFullyHandled)
                    {
                        if (ct.IsCancellationRequested)
                            return;

                        Thread.Sleep(notification.Delay);
                        if (ct.IsCancellationRequested)
                            return;
                        notification.NotifiedOnUtc = DateTime.UtcNow;
                        _mockHub.Clients.All.SendAsync(notification.Key, notification.Body);
                        notification.NotifiedCounter++;
                        Task.Run(() => _notificationRepository.Update(notification));
                    }
                }
            });
        }
        public void Stop()
        {
            _notificationsCancelationTokenSource?.Cancel();
        }
    }
}