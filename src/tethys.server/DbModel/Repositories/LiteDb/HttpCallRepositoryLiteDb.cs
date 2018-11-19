using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tethys.Server.Models;
using Tethys.Server.Services;
using Tethys.Server.Services.HttpCalls;

namespace Tethys.Server.DbModel.Repositories.LiteDb
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

        public void Update(HttpCall httpCall)
        {
            _liteDbUnitOfWork.Update(httpCall);
        }

        public void FlushUnhandled()
        {
            var notHandledOrFlushed = _liteDbUnitOfWork.Query<HttpCall>(hc => !hc.WasFullyHandled && !hc.Flushed);
            foreach (var nf in notHandledOrFlushed)
            {
                nf.FlushedOnUtc = DateTime.UtcNow;
                nf.Flushed = true;
                _liteDbUnitOfWork.Update(nf);
            }
        }

        public IEnumerable<HttpCall> GetAll(Func<HttpCall, bool> filter = null)
        {
            return filter == null ? _liteDbUnitOfWork.GetAll<HttpCall>() : _liteDbUnitOfWork.Query<HttpCall>(filter);
        }
    }
}