using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.SignalR;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.Hubs;
using Tethys.WebApi.Models;
using Tethys.WebApi.Services;

namespace Tethys.WebApi.Controllers
{
    [Route(Consts.MockControllerRoute)]
    public class MockController : Controller
    {
        #region CTOR

        public MockController(IHttpCallRepository httpCallRepository, IHubContext<MockHub> mockHub, INotificationService notificationeService)
        {
            _httpCallRepository = httpCallRepository;
            _mockHub = mockHub;
            _notificationeService = notificationeService;
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var actualRequest = await BuildActualRequest();
            var httpCall = await Task.FromResult(_httpCallRepository.GetNextHttpCall());

            //TODO: send via web socket
            if (httpCall == null)
                return new NotFoundObjectResult(new
                {
                    message = "No corrosponding HttpCall object",
                    requestDetails = new
                    {
                        headers = actualRequest.Headers,
                        httpMethod = actualRequest.HttpMethod,
                        path = actualRequest.Resource,
                        query = actualRequest.Query,
                        body = actualRequest.Body
                    }
                });
            await ReportViaWebSocket(actualRequest, httpCall.Request);

            httpCall.CallsCounter++;
            httpCall.WasFullyHandled = httpCall.CallsCounter == httpCall.AllowedCallsNumber;
            httpCall.HandledOnUtc = DateTime.UtcNow;

            _httpCallRepository.Update(httpCall);

            //delay before response
            Thread.Sleep(httpCall.Response.Delay);
            return httpCall.Response.ToHttpResponseMessage();
        }

        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post()
        {
            await Task.Run(() =>
            {
                _notificationeService.Stop();

                _httpCallRepository.FlushUnhandled();
            });
            return Ok();
        }

        [HttpPost("setup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] IEnumerable<HttpCall> httpCalls)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Bad or missing data",
                    errors = ModelState.Values.Select(x => x.Errors),
                    data = httpCalls
                });

            await Task.Run(() =>
            {
                var enumerable = httpCalls as HttpCall[] ?? httpCalls.ToArray();

                foreach (var hc in enumerable)
                {
                    hc.WasFullyHandled = false;
                    hc.CreatedOnUtc = DateTime.UtcNow;
                    hc.AllowedCallsNumber = Math.Max(1, hc.AllowedCallsNumber);
                    hc.CallsCounter = 0;
                }

                _httpCallRepository.Create(enumerable);
            });
            return new ObjectResult(httpCalls) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPost("push")]
        public IActionResult Push([FromBody] IEnumerable<PushNotification> notifications)
        {
            if (notifications == null || !notifications.Any())
                return new ObjectResult("No notifications sent to server")
                {
                    StatusCode = StatusCodes.Status406NotAcceptable
                };

            _notificationeService.NotifyAsync(notifications);

            return Accepted();
        }

        private async Task ReportViaWebSocket(Request actualRequest, Request expectedRequest)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Incoming Http Request:");
            sb.AppendLine(actualRequest.ToReportFormat().Replace("\n", "\t\n"));
            sb.AppendLine("Expected Http Request:");
            sb.AppendLine(expectedRequest.ToReportFormat().Replace("\n", "\t\n"));

            sb.AppendLine("Start comparing incoming request");
            await _mockHub.Clients.All.SendAsync("tethys-log", sb.ToString());
        }

        private async Task<Request> BuildActualRequest()
        {
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var headerDictionary = Request.Headers.ToDictionary(s => s.Key, s => s.Value);
            var originalRequest = Request.HttpContext.Items[Consts.OriginalRequest] as OriginalRequest;
            return new Request
            {
                HttpMethod = Enum.Parse<HttpMethod>(originalRequest.HttpMethod, true),
                Resource = originalRequest.Path,
                Query = originalRequest.QueryString,
                Body = body,
                Headers = headerDictionary as IDictionary<string, string>
            };
        }

        #region Fields

        private readonly IHttpCallRepository _httpCallRepository;
        private readonly IHubContext<MockHub> _mockHub;
        private readonly INotificationService _notificationeService;

        #endregion
    }
}