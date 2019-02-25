using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tethys.Server.Models;

namespace Tethys.Server.Services.HttpCalls
{
    public interface IHttpCallService
    {
        Task CreateOrUpdateHttpCalls(IEnumerable<HttpCall> httpCalls);
        Task<HttpCall> GetNextHttpCall(Request request);
        void Reset();
    }
}