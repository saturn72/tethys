using System.Collections.Generic;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories.LiteDb
{
    public class NotificationRepositoryLiteDb : INotificationRepository
    {
        #region Fields

        private readonly UnitOfWorkLiteDb _liteDbUnitOfWork;

        #endregion

        #region CTOR

        public NotificationRepositoryLiteDb(UnitOfWorkLiteDb liteDbUnitOfWork)
        {
            _liteDbUnitOfWork = liteDbUnitOfWork;
        }
        #endregion

        public void Create(IEnumerable<PushNotification> notifications)
        {
            _liteDbUnitOfWork.Create(notifications);
        }

        public void Update(PushNotification notification)
        {
            _liteDbUnitOfWork.Update(notification);            
        }
    }
}