using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tethys.Server.Models;
using Tethys.Server.Services;
using Tethys.Server.Services.HttpCalls;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Controllers
{
    [Route(Consts.MockControllerRoute)]
    public class MockController : Controller
    {
        #region CTOR

        public MockController(
            IHttpCallService httpCallService, INotificationService notificationeService,
             IRequestResponseCoupleService requestResponseCoupleService, IFileUploadManager fileuploadManager)
        {
            _httpCallService = httpCallService;
            _notificationeService = notificationeService;
            _reqRescoupleService = requestResponseCoupleService;
            _fileuploadManager = fileuploadManager;
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var httpCall = await _httpCallService.GetNextHttpCall(Request);

            //TODO: send via web socket
            if (httpCall == null)
            {
                var originalRequest = Request.HttpContext.Items[Consts.OriginalRequest] as OriginalRequest;
                return new NotFoundObjectResult(new
                {
                    message = "No corrosponding HttpCall object",
                    requestDetails = new
                    {
                        headers = originalRequest.Headers,
                        httpMethod = originalRequest.HttpMethod,
                        path = originalRequest.Path,
                        query = originalRequest.QueryString,
                        body = originalRequest.Body
                    }
                });
            }
            //delay before response
            Thread.Sleep(httpCall.Response.Delay);
            return httpCall.Response.ToHttpResponseMessage();
        }

        [HttpPost(Consts.MockUploadRoute)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Upload()
        {
            var files = Request.Form.Files;
            if (files.Count == 0 || files.All(f => f.Length == 0))
                return BadRequest("Missing or empty file(s) content");

            var streams = getFilesAsStreamAsync();
            await _fileuploadManager.LoadSequenceFromStream(streams);

            return Accepted();

            IEnumerable<Stream> getFilesAsStreamAsync()
            {
                var res = new List<Stream>();

                foreach (var f in files)
                {
                    if (f.Length <= 0)
                        continue;
                    res.Add(f.OpenReadStream());
                }
                return res;
            }
        }

        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post()
        {
            await Task.Run(async () =>
            {
                _notificationeService.Stop();

                _httpCallService.Register();
                await _reqRescoupleService.DeleteAllAsync();
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

            await _httpCallService.Register(httpCalls);

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



        #region Fields

        private readonly INotificationService _notificationeService;
        private readonly IRequestResponseCoupleService _reqRescoupleService;
        private readonly IFileUploadManager _fileuploadManager;
        private readonly IHttpCallService _httpCallService;

        #endregion
    }
}