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
        public async Task<HttpCall> GetHttpCallByRequest(Request request)
        {
            if (request == null || !request.BucketId.HasValue())
                return null;

            var filter = new Func<HttpCall, bool>(hc =>
            {
                var res = !hc.Flushed &&
                hc.BucketId.Equals(request.BucketId, StringComparison.InvariantCultureIgnoreCase) &&
                !hc.WasFullyHandled &&
                hc.Request.HttpMethod.Split('|').Any(hm => hm.Trim().Equals(request.HttpMethod, StringComparison.InvariantCultureIgnoreCase)) &&
                Regex.IsMatch(request.Resource, hc.Request.Resource, RegexOptions.IgnoreCase);

                return res;
            });

            var httpCalls = await Task.Run(() => _httpCallRepository.GetAll(filter));
            if (httpCalls.IsNullOrEmpty())
                return null;

            var lastHttpCall = httpCalls.OrderByDescending(h => h.Id).First();
            // await ReportViaWebSocket(httpCall.Request, httpCall.Request);
            lastHttpCall.CallsCounter++;
            lastHttpCall.WasFullyHandled = lastHttpCall.CallsCounter == lastHttpCall.AllowedCallsNumber;
            lastHttpCall.HandledOnUtc = DateTime.UtcNow;
            lastHttpCall.UpdatedOnUtc = DateTime.UtcNow;

            await Task.Run(() => _httpCallRepository.Update(lastHttpCall));
            return lastHttpCall;
        }

        public async Task<ServiceOperationResult> AddHttpCalls(IEnumerable<HttpCall> httpCalls)
        {
            if (httpCalls.IsNullOrEmpty())
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
                    hc.UpdatedOnUtc = DateTime.UtcNow;
                    hc.AllowedCallsNumber = hc.AllowedCallsNumber == 0 ? 1024 : hc.AllowedCallsNumber;
                    hc.CallsCounter = 0;
                }

                _httpCallRepository.Create(httpCalls);
            });


            if (httpCalls.All(hc => hc.Id > 0))
                return new ServiceOperationResult
                {
                    Status = ServiceOperationStatus.Success
                };


            var status = httpCalls.All(hc => hc.Id == 0) ? ServiceOperationStatus.Fail : ServiceOperationStatus.Partially;
            var msgBase = $" httpcalls from collection named: {nameof(httpCalls)} creation failed";

            return new ServiceOperationResult
            {
                Status = status,
                Message = status == ServiceOperationStatus.Fail ? "All" : "Some" + msgBase
            };
        }

        public void Reset()
        {
            _httpCallRepository.FlushUnhandled();
        }

    }
}