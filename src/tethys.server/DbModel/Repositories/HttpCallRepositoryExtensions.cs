using System;
using System.Linq;
using System.Linq.Expressions;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories
{
    public static class HttpCallRepositoryExtensions
    {
        private static readonly ISpecification<HttpCall> GetNotExecutedHttpCallsSpec = 
            new SimpleSpecification<HttpCall>(hc => !hc.WasFullyHandled && !hc.Flushed, new Expression<Func<HttpCall, object>>[]
        {
            hc => hc.Request,
            hc => hc.Response,
        });

        public static HttpCall GetNextHttpCall(this IHttpCallRepository httpCallRepository)
        {
            return httpCallRepository.GetBy(GetNotExecutedHttpCallsSpec).OrderBy(hc=>hc.Id).FirstOrDefault();
        }
    }
}