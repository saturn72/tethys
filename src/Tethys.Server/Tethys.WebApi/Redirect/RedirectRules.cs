using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Redirect
{
    public class RedirectRules
    {
        public static void RedirectRequests(RewriteContext context, TethysConfig tethysConfig)
        {
            var request = context.HttpContext.Request;

            var wsSuffixes = tethysConfig.WebSocketSuffix;
            if (RequestStartsWithSegment(request, Consts.ApiBaseUrl)
                || RequestStartsWithSegment(request, Consts.SwaggerEndPointPrefix)
                || RequestStartsWithSegment(request, Consts.SignalR)
                || wsSuffixes.Any(wss=> RequestStartsWithSegment(request, wss)))
                return;

            request.HttpContext.Items[Consts.OriginalRequest] = new OriginalRequest
            {
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                HttpMethod = request.Method
            };

            request.Path = new PathString(Consts.MockControllerRoute);
        }

        private static bool RequestStartsWithSegment(HttpRequest request, string segment)
        {
            return request.Path.StartsWithSegments(new PathString(segment));
        }
    }
}