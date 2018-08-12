using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<HttpCall> GetNextHttpCall(HttpRequest request)
        {
            var originalRequest = WrapOriginalRequest(request);
            var httpCall = _httpCallRepository.GetNextHttpCall();
            if (httpCall == null)
                return null;

            // await ReportViaWebSocket(httpCall.Request, httpCall.Request);

            httpCall.CallsCounter++;
            httpCall.WasFullyHandled = httpCall.CallsCounter == httpCall.AllowedCallsNumber;
            httpCall.HandledOnUtc = DateTime.UtcNow;

            _httpCallRepository.Update(httpCall);
            return httpCall;
        }

        public async Task Register(IEnumerable<HttpCall> httpCalls)
        {
            if (httpCalls == null || !httpCalls.Any())
                return;

            await Task.Run(() =>
            {
                foreach (var hc in httpCalls)
                {
                    hc.WasFullyHandled = false;
                    hc.CreatedOnUtc = DateTime.UtcNow;
                    hc.AllowedCallsNumber = Math.Max(1, hc.AllowedCallsNumber);
                    hc.CallsCounter = 0;
                }

                _httpCallRepository.Create(httpCalls);
            });
        }

        public void Register()
        {
            _httpCallRepository.FlushUnhandled();
        }
        #region Utilities
        private async Task<Request> WrapOriginalRequest(HttpRequest request)
        {
            string body;
            using (var reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var headerDictionary = request.Headers.ToDictionary(s => s.Key, s => s.Value);
            var originalRequest = request.HttpContext.Items[Consts.OriginalRequest] as OriginalRequest;
            return new Request
            {
                HttpMethod = originalRequest.HttpMethod,
                Resource = originalRequest.Path,
                Query = originalRequest.QueryString,
                Body = body,
                Headers = headerDictionary as IDictionary<string, string>
            };
        }

        // private async Task ReportViaWebSocket(OriginalRequest originalRequest, Request expectedRequest)
        // {
        //     // var sb = new StringBuilder();
        //     // sb.AppendLine("Incoming Http Request:");
        //     // sb.AppendLine(originalRequest.ToReportFormat().Replace("\n", "\t\n"));
        //     // sb.AppendLine("Expected Http Request:");
        //     // sb.AppendLine(expectedRequest.ToReportFormat().Replace("\n", "\t\n"));

        //     // sb.AppendLine("Start comparing incoming request");
        //     // await _notificationPublisher.ToServerUnderTestClients("tethys-log", sb.ToString());
        // }

        #endregion
    }
}