using System;
using System.Collections.Generic;
using Tethys.Server.Models;

namespace Tethys.Server.Services.HttpCalls
{
    public interface IHttpCallRepository
    {
        void Create(IEnumerable<HttpCall> httpCalls);
        void Update(HttpCall httpCall);
        void FlushUnhandled();
        IEnumerable<HttpCall> GetAll(Func<HttpCall, bool> filter = null);
    }
}
