using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tethys.Server.Controllers;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.Models;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Services.HttpCalls
{
    public class HttpCallService : IHttpCallService
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IHttpCallRepository _httpCallRepository;

        public HttpCallService(INotificationPublisher notificationPublisher, IHttpCallRepository httpCallRepository)
        {
            _notificationPublisher = notificationPublisher;
            _httpCallRepository = httpCallRepository;
        }
        public async Task<HttpCall> GetHttpCall(Request request)
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

            Task.Run(() => _httpCallRepository.Update(httpCall));
            return httpCall;
        }

        public async Task<ServiceOperationResult> AddHttpCalls(IEnumerable<HttpCall> httpCalls)
        {
            if (httpCalls == null || !httpCalls.Any())
                return new ServiceOperationResult
                {
                    Status = ServiceOperationStatus.Fail,
                    Message = $"Fail to create httpCalls - null or empty collection was sent: {nameof(httpCalls)}"
                };
            if (httpCalls.Any(hc => !hc.BucketId.HasValue()))
                return new ServiceOperationResult
                {
                    Status = ServiceOperationStatus.Fail,
                    Message = "Missing bucketId for some or all of httpcalls"
                };

            await Task.Run(() =>
            {
                foreach (var hc in httpCalls)
                {
                    hc.WasFullyHandled = false;
                    hc.CreatedOnUtc = DateTime.UtcNow;
                    hc.AllowedCallsNumber = hc.AllowedCallsNumber == 0 ? 1024 : hc.AllowedCallsNumber;
                    hc.CallsCounter = 0;
                }

                _httpCallRepository.Create(httpCalls);
            });


            if (httpCalls.All(hc => hc.Id > 0))
                return new ServiceOperationResult
                {
                    Status = ServiceOperationStatus.Success,
                };

            var msg = (httpCalls.All(hc => hc.Id == 0) ? "All" : "Some") + $" httpcalls from collection named: {nameof(httpCalls)} creation failed";
            return new ServiceOperationResult
            {
                Status = ServiceOperationStatus.Fail,
                Message = msg
            };
        }

        public void Reset()
        {
            _httpCallRepository.FlushUnhandled();
        }

    }
}