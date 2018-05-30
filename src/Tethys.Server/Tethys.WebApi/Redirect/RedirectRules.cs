using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Redirect
{
    public class RedirectRules
    {
        public static void RedirectRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            // Because we're redirecting back to the same app, stop 
            // processing if the request has already been redirected
            if (RequestStartsWithSegment(request, Consts.ApiBaseUrl)
                || RequestStartsWithSegment(request, Consts.SwaggerEndPointPrefix)
                || RequestStartsWithSegment(request, Consts.WebSocketRoutePrefix))
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