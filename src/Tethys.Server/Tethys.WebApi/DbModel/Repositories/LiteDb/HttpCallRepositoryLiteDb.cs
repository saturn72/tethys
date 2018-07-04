using System;
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

        public void Create(IEnumerable<HttpCall> httpCalls)
        {
            _liteDbUnitOfWork.Create(httpCalls);
        }
        public IEnumerable<HttpCall> GetBy(ISpecification<HttpCall> spec)
        {
            return _liteDbUnitOfWork.Query(spec);
        }

        public void Update(HttpCall httpCall)
        {
            _liteDbUnitOfWork.Update(httpCall);
        }

        public void FlushUnhandled()
        {
            var notHandledOrFlushed = _liteDbUnitOfWork.Query<HttpCall>(hc => !hc.WasFullyHandled && !hc.Flushed );
            foreach (var nf in notHandledOrFlushed)
            {
                nf.FlushedOnUtc = DateTime.UtcNow;
                nf.Flushed = true;
                _liteDbUnitOfWork.Update(nf);
            }
        }
    }
}