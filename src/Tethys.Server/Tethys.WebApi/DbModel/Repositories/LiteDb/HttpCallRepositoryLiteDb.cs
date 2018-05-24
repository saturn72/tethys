using System.Collections.Generic;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories.LiteDb
{
    public class HttpCallRepositoryLiteDb : IHttpCallRepository
    {
        #region Fields

        private readonly UnitOfWorkLiteDb _liteDbUnitOfWork;

        #endregion

        #region CTOR

        public HttpCallRepositoryLiteDb(UnitOfWorkLiteDb liteDbUnitOfWork)
        {
            _liteDbUnitOfWork = liteDbUnitOfWork;
        }

        #endregion

        public void Insert(HttpCall httpCall)
        {
            httpCall.WasHandled = false;
            _liteDbUnitOfWork.Insert(httpCall);
        }
        public IEnumerable<HttpCall> GetBy(ISpecification<HttpCall> spec)
        {
            return _liteDbUnitOfWork.Query(spec);
        }

        public void Update(HttpCall httpCall)
        {
            _liteDbUnitOfWork.Update(httpCall);
        }
    }
}