using System.Collections.Generic;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories.LiteDb
{
    public class RequestResponseCoupleRepositoryLiteDb : IRequestResponseCoupleRepository
    {
        #region Fields

        private readonly UnitOfWorkLiteDb _liteDbUnitOfWork;

        #endregion

        #region CTOR

        public RequestResponseCoupleRepositoryLiteDb(UnitOfWorkLiteDb liteDbUnitOfWork)
        {
            _liteDbUnitOfWork = liteDbUnitOfWork;
        }
        #endregion
        public void Create(RequestResponseCouple requestResponseCouple)
        {
            _liteDbUnitOfWork.Create(requestResponseCouple);
        }

        public IEnumerable<RequestResponseCouple> GetAll()
        {
            return _liteDbUnitOfWork.GetAll<RequestResponseCouple>();
        }

        public void Update(RequestResponseCouple requestResponseCouple)
        {
            _liteDbUnitOfWork.Update(requestResponseCouple);
        }
    }
}