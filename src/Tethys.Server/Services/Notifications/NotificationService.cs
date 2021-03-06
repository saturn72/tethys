﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceStack;
using ServiceStack.Text;
using Tethys.Server.DbModel;
using Tethys.Server.Hubs;
using Tethys.Server.Models;

namespace Tethys.Server.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        protected static CancellationTokenSource _notificationsCancelationTokenSource;
        private readonly IRepository<PushNotification> _notificationRepository;
        private readonly INotificationPublisher _publisher;

        public NotificationService(INotificationPublisher publisher, IRepository<PushNotification> notificationRepository)
        {
            _publisher = publisher;
            _notificationRepository = notificationRepository;
        }

        public async Task NotifyAsync(IEnumerable<PushNotification> notifications)
        {
            //stop previous notifications
            Stop();

            if (notifications == null || !notifications.Any())
                return;

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
            await Task.Run((Action)(() =>
            {
                foreach (var notification in notifications)
                {
                    while (!notification.WasFullyHandled)
                    {
                        if (ct.IsCancellationRequested)
                            return;

                        Thread.Sleep((int)notification.Delay);
                        if (ct.IsCancellationRequested)
                            return;
                        notification.NotifiedOnUtc = DateTime.UtcNow;
                        var body = JsonObject.Parse(notification.Body);

                        _publisher.ToClients(notification.Key, body);
                        notification.NotifiedCounter++;
                        _notificationRepository.Update(notification);
                    }
                }
            }));
        }
        public virtual void Stop()
        {
            _notificationsCancelationTokenSource?.Cancel();
        }
    }
}