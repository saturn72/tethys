using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using Tethys.Server.Models;
using Tethys.Server.Services;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Middlewares
{
    /// <summary>
    /// see: https://www.carlrippon.com/adding-useful-information-to-asp-net-core-web-api-serilog-logs/
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        #region Fields
        private readonly RequestDelegate _next;
        private readonly IRequestResponseCoupleService _requestResponseCoupleService;
        private readonly INotificationPublisher _notificationPublisher;
        private IEnumerable<string> _webSocketSuffix;

        #endregion
        #region ctor
        public RequestResponseLoggingMiddleware(RequestDelegate next, IRequestResponseCoupleService requestResponseCoupleService,
        INotificationPublisher notificationPublisher, TethysConfig tethysConfig)
        {
            _next = next;
            _requestResponseCoupleService = requestResponseCoupleService;
            _notificationPublisher = notificationPublisher;
            _webSocketSuffix = tethysConfig.WebSocketSuffix;
        }
        #endregion

        public async Task Invoke(HttpContext context)
        {
            if (RedirectRules.ShouldInterceptRequestByPath(context.Request, _webSocketSuffix))
                await LogRequestResponse(context);
            else
                await _next(context);
        }

        private async Task LogRequestResponse(HttpContext context)
        {
            var request = context.Request;
            var reqRes = await ExtractRequestAsync(request);
            await _requestResponseCoupleService.Create(reqRes);

            var response = context.Response;

            using (var resMS = new MemoryStream())
            {
                var originalResponseBodyReference = response.Body;
                response.Body = resMS;

                await _next(context);

                response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                reqRes.Response = new Response
                {
                    HttpStatusCode = response.StatusCode,
                    Body = responseBody
                };
                await resMS.CopyToAsync(originalResponseBodyReference);
            }

            await _requestResponseCoupleService.Update(reqRes);
            var json = JsonConvert.SerializeObject(reqRes);
            _notificationPublisher.ToServerUnderTestClients(TethysNotificationKeys.NewRequestResponseCouple, json);
        }

        private async Task<RequestResponseCouple> ExtractRequestAsync(HttpRequest request)
        {
            request.EnableRewind();
            var reqBody = request.Body;
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            reqBody.Seek(0, SeekOrigin.Begin);
            request.Body = reqBody;

            return new RequestResponseCouple
            {
                Request = new Request
                {
                    Resource = request.Path,
                    Query = request.QueryString.ToString(),
                    HttpMethod = request.Method,
                    Body = requestBody,
                    Headers = request.Headers.ToDictionary(s => s.Key, s => s.Value.AsEnumerable())
                }
            };
        }
    }
}