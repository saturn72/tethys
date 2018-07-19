using System.Collections.Generic;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories
{
    public interface IHttpCallRepository
    {
        void Create(IEnumerable<HttpCall> httpCalls);
        IEnumerable<HttpCall> GetBy(ISpecification<HttpCall> spec);
        void Update(HttpCall httpCall);
        void FlushUnhandled();
        IEnumerable<HttpCall> GetAll();
    }
}
