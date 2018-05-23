using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Controllers
{
    [Route(Consts.MockControllerRoute)]
    public class MockController : Controller
    {
        #region Fields

        private readonly IHttpCallRepository _httpCallRepository;

        #endregion

        #region CTOR

        public MockController(IHttpCallRepository httpCallRepository)
        {
            _httpCallRepository = httpCallRepository;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var actualRequest = await BuildActualRequest();
            

            var httpCall = await Task.FromResult(_httpCallRepository.GetNextHttpCall());

            //AssertRequest(httpcal)
            //TODO: send via web socket
            if(httpCall == null)
                return new BadRequestObjectResult(new
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

            return httpCall.Response.ToHttpResponseMessage();
        }

        private async Task<Request> BuildActualRequest()
        {
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var headerDictionary = Request.Headers.ToDictionary(s => s.Key, s => s.Value);
            return new Request
            {
                HttpMethod = (HttpMethod)Request.HttpContext.Items[Consts.OriginalRequestHttpMethod],
                Resource = Request.HttpContext.Items[Consts.OriginalRequestPath].ToString(),
                Query = Request.HttpContext.Items[Consts.OriginalRequestQuery].ToString(),
                Body = body,
                Headers = headerDictionary as IDictionary<string, string>
            };
        }
    }
}
