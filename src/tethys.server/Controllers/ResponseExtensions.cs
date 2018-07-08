using Microsoft.AspNetCore.Mvc;
using Tethys.Server.Models;

namespace Tethys.Server.Controllers
{
    public static class ResponseExtensions {

        public static IActionResult ToHttpResponseMessage(this Response response)
        {
            return string.IsNullOrEmpty(response.Body) ||
                   string.IsNullOrWhiteSpace(response.Body)
                ? new StatusCodeResult(response.HttpStatusCode) as IActionResult
                : new ObjectResult(response.Body) {StatusCode = response.HttpStatusCode};
        }
    }
}