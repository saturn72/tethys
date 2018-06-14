using System.Collections.Generic;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories
{
    public interface IHttpCallRepository
    {
        void Insert(IEnumerable<HttpCall> httpCalls);
        IEnumerable<HttpCall> GetBy(ISpecification<HttpCall> spec);
        void Update(HttpCall httpCall);
        void FlushUnhandled();
    }
}
