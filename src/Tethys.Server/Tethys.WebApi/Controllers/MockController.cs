using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Text;
using Tethys.WebApi.DbModel;
using Tethys.WebApi.DbModel.Repositories;

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
            var origHttpMethod = Request.HttpContext.Items[Consts.OriginalRequestHttpMethod];
            var origrequestPath = Request.HttpContext.Items[Consts.OriginalRequestPath];
            var origQuery = Request.HttpContext.Items[Consts.OriginalRequestQuery];
            string body = null;
            //read request's body
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var headerDictionary = Request.Headers.ToDictionary(s=>s.Key, s=>s.Value);
            var headers = JsonSerializer.SerializeToString(headerDictionary);

            var httpCall = await Task.FromResult(_httpCallRepository.GetNextHttpCall());

            //TODO: send via web socket
            if(httpCall == null)
                return new BadRequestObjectResult(new
                {
                    message = "No corrosponding HttpCall object",
                    requestDetails = new
                    {
                        @headers,
                        httpMethod = origHttpMethod,
                        path = origrequestPath,
                        query = origQuery,
                        body,
                    }
                });

            return httpCall.Response.ToHttpResponseMessage();
        }
    }
}
