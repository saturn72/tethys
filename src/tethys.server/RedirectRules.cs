using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Tethys.Server.Models;

namespace Tethys.Server
{
    public class RedirectRules
    {
        public static void RedirectRequests(HttpRequest request, TethysConfig tethysConfig)
        {
            //if tethys requests - continue
            if (RequestStartsWithSegment(request, Consts.ApiBaseUrl)
                || RequestStartsWithSegment(request, Consts.SwaggerEndPointPrefix)
                || RequestStartsWithSegment(request, Consts.TethysWebSocketPath))
                return;

            request.HttpContext.Items[Consts.OriginalRequest] = new OriginalRequest
            {
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                HttpMethod = request.Method
            };

            BuildRedirectLogic(tethysConfig, request);
        }

        private static void BuildRedirectLogic(TethysConfig tethysConfig, HttpRequest request)
        {
            var isWebSocketRequest = tethysConfig
                .WebSocketSuffix
                .Any(wss => RequestStartsWithSegment(request, wss));

            var path = isWebSocketRequest ? Consts.TethysWebSocketPath : Consts.MockControllerRoute;
            if (isWebSocketRequest)
            {
                if (request.Path.Value.EndsWith(Consts.TethysWebSocketPathNegotiate,
                                   StringComparison.InvariantCultureIgnoreCase))
                    path += Consts.TethysWebSocketPathNegotiate;
            }
            else
            {
                request.Method = HttpMethods.Get;
            }
            request.Path = path;
        }

        private static bool RequestStartsWithSegment(HttpRequest request, string segment)
        {
            try
            {
                return request.Path.StartsWithSegments(new PathString(segment));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}