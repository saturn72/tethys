using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tethys.Server.Models;

namespace Tethys.Server.Services.HttpCalls
{
    public static class HttpCallServiceExtensions
    {
        public static async Task<HttpCall> GetNextHttpCall(this IHttpCallService httpCallService, HttpRequest httpRequest)
        {
            var request = await WrapOriginalRequest(httpRequest);
            return await httpCallService.GetHttpCall(request);

        }
        private static async Task<Request> WrapOriginalRequest(HttpRequest request)
        {
            string body;
            using (var reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var originalRequest = request.HttpContext.Items[Consts.OriginalRequest] as Request;
            return new Request
            {
                HttpMethod = originalRequest.HttpMethod,
                Resource = originalRequest.Resource,
                Query = originalRequest.Query,
                Body = body,
                Headers = request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Aggregate((x, y) => x + ";" + y))
            };
        }
    }
}