using System.Collections.Generic;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories
{
    public interface INotificationRepository
    {
        void Create(IEnumerable<PushNotification> notifications);
        void Update(PushNotification notification);
    }
}