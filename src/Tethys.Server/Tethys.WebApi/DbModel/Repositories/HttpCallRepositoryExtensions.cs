using System;
using System.Linq;
using System.Linq.Expressions;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.DbModel.Repositories
{
    public static class HttpCallRepositoryExtensions
    {
        private static readonly ISpecification<HttpCall> GetNotExecutedHttpCallsSpec = new SimpleSpecification<HttpCall>(hc => !hc.WasExecuted, new Expression<Func<HttpCall, object>>[]
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