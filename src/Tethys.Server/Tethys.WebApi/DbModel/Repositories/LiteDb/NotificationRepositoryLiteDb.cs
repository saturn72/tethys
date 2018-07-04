using System.Collections.Generic;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories.LiteDb
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