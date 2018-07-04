using System.Collections.Generic;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories
{
    public interface INotificationRepository
    {
        void Create(IEnumerable<PushNotification> notifications);
        void Update(PushNotification notification);
    }
}