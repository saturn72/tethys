using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Shouldly;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.Hubs;
using Tethys.Server.Models;
using Tethys.Server.Services.Notifications;
using Xunit;

namespace Tethys.Server.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public void NotificationService_Stop()
        {
            var ns = new NotificationServiceTestObject(null, null);
            ns.StopMethodWasCalled.ShouldBeFalse();
            ns.Stop();
            ns.StopMethodWasCalled.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(NotificationService_Notifyt_EmptyCollection_Data))]
        public async Task NotificationService_Notify_EmptyCollection(IEnumerable<PushNotification> notifications)
        {
            var nRepo = new Mock<INotificationRepository>();
            var ns = new NotificationService(null, nRepo.Object);
            await ns.NotifyAsync(notifications);
            nRepo.Verify(n => n.Create(It.IsAny<IEnumerable<PushNotification>>()), Times.Never);
        }
        public static IEnumerable<object[]> NotificationService_Notifyt_EmptyCollection_Data
        => new[]{
            new[]{null as IEnumerable<PushNotification>},
            new[]{new PushNotification[]{}},
        };

        [Fact]
        public async Task NotificationService_Notify()
        {
            string key = "some-key",
            body = "some-body";
            var notifications = new[]{
                new PushNotification{Key=key+ "1", Body = body, NotifyTimes = 1, NotifiedCounter = 1},
                new PushNotification{Key=key+"2", Body = body, NotifiedCounter = 1},
                new PushNotification{Key=key + "3", Body = body, NotifyTimes = 3, NotifiedCounter = 10},
            };

            var np = new Mock<INotificationPublisher>();
            var nRepo = new Mock<INotificationRepository>();
            var ns = new NotificationService(np.Object, nRepo.Object);
            await ns.NotifyAsync(notifications);

            //verify all notifications were set properly
            nRepo.Verify(n => n.Create(It.Is<IEnumerable<PushNotification>>(nItems =>
                nItems.Count() == notifications.Length
                && nItems.ToList().GetRange(0, notifications.Length - 1).All(i => i.NotifyTimes == 1)
                && nItems.Last().NotifyTimes == (uint)3
                )), Times.Once);

            //notification
            foreach (var ntf in notifications)
            {
                np.Verify(n => n.ToAll(It.Is<string>(k => ntf.Key == k), It.Is<string>(b => ntf.Body == b)), Times.Exactly((int)ntf.NotifyTimes));

                for (var i = 1; i <= ntf.NotifiedCounter; i++)
                    nRepo.Verify(n => n.Update(It.Is<PushNotification>(pn => pn.Key == ntf.Key)), Times.AtLeastOnce);
            }
        }
    }
    public class NotificationServiceTestObject : NotificationService
    {
        public NotificationServiceTestObject(INotificationPublisher publisher, INotificationRepository notificationRepository)
        : base(publisher, notificationRepository)
        {
        }

        public override void Stop()
        {
            StopMethodWasCalled = true;
            base.Stop();
        }
        public bool StopMethodWasCalled
        {
            get; private set;
        }
    }
}