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
    [Route(Consts.HttpCallControllerRoute)]
    public class HttpCallController : Controller
    {
        #region CTOR

        public HttpCallController(
            IHttpCallService httpCallService, IRequestResponseCoupleService requestResponseCoupleService, IFileUploadManager fileuploadManager)
        {
            _httpCallService = httpCallService;
            _reqRescoupleService = requestResponseCoupleService;
            _fileuploadManager = fileuploadManager;
        }

        #endregion

        /// <summary>
        /// Gets next http-call in sequence
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var originalRequest = Request.HttpContext.Items[Consts.OriginalRequest] as Request;
            var httpCall = await _httpCallService.GetNextHttpCall(originalRequest);

            //TODO: send via web socket
            if (httpCall == null)
            {
                var resBody = new
                {
                    message = "No corrosponding HttpCall object",
                    requestDetails = new
                    {
                        headers = originalRequest.Headers,
                        httpMethod = originalRequest.HttpMethod,
                        path = originalRequest.Resource,
                        query = originalRequest.Query,
                        body = originalRequest.Body
                    }
                };
                return StatusCode(StatusCodes.Status503ServiceUnavailable, resBody);
            }

            //delay before response
            Thread.Sleep(httpCall.Response.Delay);
            var res = httpCall.Response.ToActionResult();

            var headers = httpCall.Response.Headers;
            if (headers != null && headers.Any())
                foreach (var h in headers)
                    Request.HttpContext.Response.Headers[h.Key] = h.Value;
            return res;
        }

        /// <summary>
        /// Uploads files contains http-call sequence to server
        /// </summary>
        /// <returns>StatusCodes.Status202Accepted</returns>
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

        /// <summary>
        /// Resets server's calls.
        /// Note: this command deletes all http-calls
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAll()
        {
            await Task.Run(async () =>
            {
                _httpCallService.Reset();
                await _reqRescoupleService.DeleteAllAsync();
            });
            return NoContent();
        }

        /// <summary>
        /// setup array of http-calls
        /// </summary>
        /// <param name="httpCalls"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] IEnumerable<HttpCall> httpCalls)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Bad or missing data",
                    errors = ModelState.Values.Select(x => x.Errors),
                    data = httpCalls
                });

            await _httpCallService.AddHttpCalls(httpCalls);

            return new ObjectResult(httpCalls) { StatusCode = StatusCodes.Status201Created };
        }
        #region Fields

        private readonly IRequestResponseCoupleService _reqRescoupleService;
        private readonly IFileUploadManager _fileuploadManager;
        private readonly IHttpCallService _httpCallService;

        #endregion
    }
}