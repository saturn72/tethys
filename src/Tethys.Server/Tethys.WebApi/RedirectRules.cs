﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace Tethys.WebApi
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

            request.HttpContext.Items[Consts.OriginalRequestPath] = request.Path;
            request.HttpContext.Items[Consts.OriginalRequestQuery] = request.QueryString;
            request.HttpContext.Items[Consts.OriginalRequestHttpMethod] = request.Method;

            request.Path = new PathString(Consts.MockControllerRoute);
        }

        private static bool RequestStartsWithSegment(HttpRequest request, string segment)
        {
            return request.Path.StartsWithSegments(new PathString(segment));
        }
    }
}