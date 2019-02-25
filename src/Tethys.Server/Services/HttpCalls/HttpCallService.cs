using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tethys.Server.Controllers;
using Tethys.Server.DbModel;
using Tethys.Server.Models;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Services.HttpCalls
{
    public class HttpCallService : IHttpCallService
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IRepository<HttpCall> _httpCallRepository;

        public HttpCallService(INotificationPublisher notificationPublisher, IRepository<HttpCall> httpCallRepository)
        {
            _notificationPublisher = notificationPublisher;
            _httpCallRepository = httpCallRepository;
        }
        public async Task<HttpCall> GetNextHttpCall(Request request)
        {
            var filter = new Func<HttpCall, bool>(hc =>
            {
                var res = !hc.Flushed &&
                !hc.WasFullyHandled &&
                hc.Request.HttpMethod.Split('|').Any(hm => hm.Trim().Equals(request.HttpMethod, StringComparison.InvariantCultureIgnoreCase)) &&
                Regex.IsMatch(request.Resource, hc.Request.Resource, RegexOptions.IgnoreCase);

                return res;
            });

            var filteredHC = _httpCallRepository.GetAll(filter);
            var httpCall = filteredHC.FirstOrDefault();
            if (httpCall == null)
                return null;
            // await ReportViaWebSocket(httpCall.Request, httpCall.Request);
            httpCall.CallsCounter++;
            httpCall.WasFullyHandled = httpCall.CallsCounter == httpCall.AllowedCallsNumber;
            httpCall.HandledOnUtc = DateTime.UtcNow;

            _httpCallRepository.Update(httpCall);
            return httpCall;
        }

        public async Task Create(IEnumerable<HttpCall> httpCalls)
        {
            if (httpCalls == null || !httpCalls.Any())
                return;

            await Task.Run(() =>
            {
                foreach (var hc in httpCalls)
                {
                    hc.WasFullyHandled = false;
                    hc.CreatedOnUtc = DateTime.UtcNow;
                    hc.AllowedCallsNumber = hc.AllowedCallsNumber == 0 ? 100 : hc.AllowedCallsNumber;
                    hc.CallsCounter = 0;
                }

                _httpCallRepository.Create(httpCalls);
            });
        }

        public void Reset()
        {
            _httpCallRepository.DeleteAll();
            // var notHandledOrFlushed = _httpCallRepository.GetAll(hc => !hc.WasFullyHandled && !hc.Flushed);
            // foreach (var nf in notHandledOrFlushed)
            // {
            //     nf.FlushedOnUtc = DateTime.UtcNow;
            //     nf.Flushed = true;
            //     _httpCallRepository.Update(nf);
            // }
        }

    }
}