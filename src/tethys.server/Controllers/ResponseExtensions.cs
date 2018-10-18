using Microsoft.AspNetCore.Mvc;
using Tethys.Server.Models;

namespace Tethys.Server.Controllers
{
    public static class ResponseExtensions
    {

        public static IActionResult ToActionResult(this Response response)
        {
            var hasBody = string.IsNullOrEmpty(response.Body) ||
                   string.IsNullOrWhiteSpace(response.Body);
            var actionResult = hasBody
                ? new StatusCodeResult(response.StatusCode) as IActionResult
                : new ObjectResult(response.Body) { StatusCode = response.StatusCode };

            return actionResult;
        }
    }
}