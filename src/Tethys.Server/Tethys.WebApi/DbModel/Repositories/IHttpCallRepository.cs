using System.Collections.Generic;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories
{
    public interface IHttpCallRepository
    {
        void Insert(HttpCall httpCall);
        IEnumerable<HttpCall> GetBy(ISpecification<HttpCall> spec);
        void Update(HttpCall httpCall);
    }
}
