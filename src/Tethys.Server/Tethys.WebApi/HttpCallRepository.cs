using System.Collections.Generic;
using System.Linq;
using Tethys.WebApi.Models;

namespace Tethys.WebApi
{
    public class HttpCallRepository
    {
        private static long _index = 1;
        private readonly ICollection<HttpCall> _httpCallList = new List<HttpCall>();
        public IEnumerable<HttpCall> GetAll => _httpCallList;

        public HttpCall GetNext => _httpCallList
            .Where(hc => !hc.WasExecuted)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        public void Insert(HttpCall httpCall)
        {
            httpCall.Id = _index++;
            _httpCallList.Add(httpCall);
        }
    }
}