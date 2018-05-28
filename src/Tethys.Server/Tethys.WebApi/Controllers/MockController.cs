﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.Models;
using Tethys.WebApi.WebSockets;

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
            ReportViaWebSocket(actualRequest, httpCall.Request);

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
            httpCall.WasHandled = true;
            httpCall.HandledOnUtc = DateTime.UtcNow;
            
            _httpCallRepository.Update(httpCall);

            //delay before response
            Thread.Sleep(httpCall.Response.Delay);
            return httpCall.Response.ToHttpResponseMessage();
        }

        private void ReportViaWebSocket(Request actualRequest, Request expectedRequest)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Incoming Http Request:");
            sb.AppendLine(actualRequest.ToReportFormat().Replace("\n", "\t\n"));
            sb.AppendLine("Expected Http Request:");
            sb.AppendLine(expectedRequest.ToReportFormat().Replace("\n", "\t\n"));

            sb.AppendLine("Start comparing incoming request");
            WebSocketOutlet.AddToBuffer(sb.ToString());
        }

        private async Task<Request> BuildActualRequest()
        {
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var headerDictionary = Request.Headers.ToDictionary(s => s.Key, s => s.Value);
            var httpContextItems = Request.HttpContext.Items;
            return new Request
            {
                HttpMethod = Enum.Parse<HttpMethod>(httpContextItems[Consts.OriginalRequestHttpMethod].ToString(),true),
                Resource = httpContextItems[Consts.OriginalRequestPath].ToString(),
                Query = httpContextItems[Consts.OriginalRequestQuery].ToString(),
                Body = body,
                Headers = headerDictionary as IDictionary<string, string>
            };
        }
    }
}
